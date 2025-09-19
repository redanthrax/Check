/**
 * Check Extension - Content Script
 * Minimal content script for Blazor WebAssembly Chrome extension
 */

// Initialize content script
console.log('Check Extension: Content script loaded on', window.location.href);

// Basic page monitoring
let pageScanned = false;

// Monitor page load and changes
function initializePageMonitoring() {
  if (pageScanned) return;
  pageScanned = true;
  
  console.log('Check Extension: Initializing page monitoring');
  
  // Basic URL analysis
  const url = window.location.href;
  const hostname = window.location.hostname;
  
  // Check for Microsoft login patterns
  const isMicrosoftDomain = hostname.includes('microsoft.com') || 
                           hostname.includes('microsoftonline.com') || 
                           hostname.includes('office.com') ||
                           hostname.includes('outlook.com');
  
  if (isMicrosoftDomain) {
    console.log('Check Extension: Microsoft domain detected:', hostname);
    addSecurityBadge('legitimate');
  }
  
  // Send page info to background script
  try {
    chrome.runtime.sendMessage({
      type: 'PAGE_SCANNED',
      url: url,
      hostname: hostname,
      title: document.title,
      isMicrosoftDomain: isMicrosoftDomain
    });
  } catch (error) {
    console.warn('Check Extension: Failed to send message to background:', error);
  }
}

// Add security badge to legitimate Microsoft pages
function addSecurityBadge(type) {
  // Remove existing badge
  const existingBadge = document.getElementById('check-security-badge');
  if (existingBadge) {
    existingBadge.remove();
  }
  
  // Create badge element
  const badge = document.createElement('div');
  badge.id = 'check-security-badge';
  badge.style.cssText = `
    position: fixed;
    top: 10px;
    right: 10px;
    background: #28a745;
    color: white;
    padding: 8px 12px;
    border-radius: 6px;
    font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif;
    font-size: 12px;
    font-weight: 600;
    box-shadow: 0 2px 8px rgba(0,0,0,0.2);
    z-index: 10000;
    transition: opacity 0.3s ease;
  `;
  
  if (type === 'legitimate') {
    badge.textContent = '✓ Verified Microsoft Site';
    badge.style.background = '#28a745';
  } else if (type === 'warning') {
    badge.textContent = '⚠ Suspicious Site';
    badge.style.background = '#ffc107';
    badge.style.color = '#000';
  } else if (type === 'danger') {
    badge.textContent = '⛔ Blocked Site';
    badge.style.background = '#dc3545';
  }
  
  document.body.appendChild(badge);
  
  // Auto-hide after 5 seconds
  setTimeout(() => {
    badge.style.opacity = '0';
    setTimeout(() => badge.remove(), 300);
  }, 5000);
}

// Listen for messages from popup/background
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
  console.log('Check Extension: Content script received message:', request);
  
  switch (request.type) {
    case 'GET_PAGE_INFO':
      sendResponse({
        success: true,
        info: {
          url: window.location.href,
          title: document.title,
          hostname: window.location.hostname
        }
      });
      break;
    case 'SHOW_BADGE':
      addSecurityBadge(request.badgeType || 'legitimate');
      sendResponse({ success: true });
      break;
    default:
      sendResponse({ success: false, message: 'Unknown request type' });
  }
  
  return true;
});

// Initialize when DOM is ready
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initializePageMonitoring);
} else {
  initializePageMonitoring();
}

// Re-initialize on navigation (for SPAs)
let lastUrl = window.location.href;
setInterval(() => {
  const currentUrl = window.location.href;
  if (currentUrl !== lastUrl) {
    lastUrl = currentUrl;
    pageScanned = false;
    setTimeout(initializePageMonitoring, 1000);
  }
}, 1000);
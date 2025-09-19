/**
 * Check Extension - Background Service Worker
 * Minimal service worker for Blazor WebAssembly Chrome extension
 */

// Keep service worker alive
self.addEventListener('install', (event) => {
  console.log('Check Extension: Service worker installed');
  self.skipWaiting();
});

self.addEventListener('activate', (event) => {
  console.log('Check Extension: Service worker activated');
  event.waitUntil(clients.claim());
});

// Handle extension icon clicks - let Blazor handle the popup
chrome.action.onClicked.addListener((tab) => {
  console.log('Check Extension: Action clicked', tab);
});

// Handle messages from content scripts and popup
chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
  console.log('Check Extension: Message received', request);
  
  switch (request.type) {
    case 'ping':
      sendResponse({ success: true, message: 'pong' });
      break;
    case 'GET_CONFIG':
      // Return basic config - actual config managed by Blazor
      sendResponse({
        success: true,
        config: {
          extensionEnabled: true,
          showNotifications: true,
          enableDebugLogging: false
        }
      });
      break;
    default:
      sendResponse({ success: false, message: 'Unknown message type' });
  }
  
  return true; // Keep message channel open for async responses
});

// Handle storage changes
chrome.storage.onChanged.addListener((changes, areaName) => {
  console.log('Check Extension: Storage changed', { changes, areaName });
});

// Handle alarms
chrome.alarms.onAlarm.addListener((alarm) => {
  console.log('Check Extension: Alarm triggered', alarm);
});

console.log('Check Extension: Background service worker loaded');
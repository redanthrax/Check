/**
 * Chrome Extension API Interop Bridge for WebAssembly
 * Provides JavaScript wrappers for Chrome extension APIs that can be called from C# WebAssembly
 */

window.chromeInterop = (function() {
    'use strict';

    // Utility function to safely handle async operations
    function safeAsync(fn) {
        return new Promise((resolve) => {
            try {
                fn(resolve);
            } catch (error) {
                console.error('Chrome interop error:', error);
                resolve(null);
            }
        });
    }

    // Storage APIs
    const storage = {
        getStorageLocal: function(key) {
            return safeAsync(resolve => {
                chrome.storage.local.get([key], result => {
                    if (chrome.runtime.lastError) {
                        console.error('Storage error:', chrome.runtime.lastError);
                        resolve('');
                        return;
                    }
                    resolve(JSON.stringify(result[key] || null));
                });
            });
        },

        getStorageLocalMultiple: function(keys) {
            return safeAsync(resolve => {
                chrome.storage.local.get(keys, result => {
                    if (chrome.runtime.lastError) {
                        console.error('Storage error:', chrome.runtime.lastError);
                        resolve('{}');
                        return;
                    }
                    resolve(JSON.stringify(result));
                });
            });
        },

        setStorageLocal: function(key, jsonValue) {
            return safeAsync(resolve => {
                const value = JSON.parse(jsonValue);
                chrome.storage.local.set({ [key]: value }, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Storage error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        setStorageLocalMultiple: function(jsonData) {
            return safeAsync(resolve => {
                const data = JSON.parse(jsonData);
                chrome.storage.local.set(data, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Storage error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        removeStorageLocal: function(key) {
            return safeAsync(resolve => {
                chrome.storage.local.remove([key], () => {
                    if (chrome.runtime.lastError) {
                        console.error('Storage error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        removeStorageLocalMultiple: function(keys) {
            return safeAsync(resolve => {
                chrome.storage.local.remove(keys, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Storage error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        clearStorageLocal: function() {
            return safeAsync(resolve => {
                chrome.storage.local.clear(() => {
                    if (chrome.runtime.lastError) {
                        console.error('Storage error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        getStorageSession: function(key) {
            return safeAsync(resolve => {
                if (!chrome.storage.session) {
                    resolve('');
                    return;
                }
                chrome.storage.session.get([key], result => {
                    if (chrome.runtime.lastError) {
                        console.error('Session storage error:', chrome.runtime.lastError);
                        resolve('');
                        return;
                    }
                    resolve(JSON.stringify(result[key] || null));
                });
            });
        },

        setStorageSession: function(key, jsonValue) {
            return safeAsync(resolve => {
                if (!chrome.storage.session) {
                    resolve();
                    return;
                }
                const value = JSON.parse(jsonValue);
                chrome.storage.session.set({ [key]: value }, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Session storage error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        removeStorageSession: function(key) {
            return safeAsync(resolve => {
                if (!chrome.storage.session) {
                    resolve();
                    return;
                }
                chrome.storage.session.remove([key], () => {
                    if (chrome.runtime.lastError) {
                        console.error('Session storage error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        getStorageManaged: function() {
            return safeAsync(resolve => {
                chrome.storage.managed.get(null, result => {
                    if (chrome.runtime.lastError) {
                        console.error('Managed storage error:', chrome.runtime.lastError);
                        resolve('{}');
                        return;
                    }
                    resolve(JSON.stringify(result || {}));
                });
            });
    },

    // File download helper
    downloadFile: function(filename, content, mimeType) {
        const blob = new Blob([content], { type: mimeType || 'application/octet-stream' });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = filename;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }
};

// Global download function for Blazor
window.downloadFile = function(filename, content, mimeType) {
    window.CheckExtension.downloadFile(filename, content, mimeType);
};

    // Runtime APIs
    const runtime = {
        sendMessage: function(jsonMessage) {
            return safeAsync(resolve => {
                const message = JSON.parse(jsonMessage);
                chrome.runtime.sendMessage(message, response => {
                    if (chrome.runtime.lastError) {
                        console.error('Runtime message error:', chrome.runtime.lastError);
                        resolve('');
                        return;
                    }
                    resolve(JSON.stringify(response || null));
                });
            });
        },

        sendMessageToTab: function(tabId, jsonMessage) {
            return safeAsync(resolve => {
                const message = JSON.parse(jsonMessage);
                chrome.tabs.sendMessage(tabId, message, response => {
                    if (chrome.runtime.lastError) {
                        console.error('Tab message error:', chrome.runtime.lastError);
                        resolve('');
                        return;
                    }
                    resolve(JSON.stringify(response || null));
                });
            });
        },

        getExtensionId: function() {
            return chrome.runtime.id || '';
        },

        getUrl: function(path) {
            return chrome.runtime.getURL(path);
        }
    };

    // Tabs APIs
    const tabs = {
        getActiveTab: function() {
            return safeAsync(resolve => {
                chrome.tabs.query({ active: true, currentWindow: true }, tabs => {
                    if (chrome.runtime.lastError) {
                        console.error('Tabs query error:', chrome.runtime.lastError);
                        resolve('');
                        return;
                    }
                    const tab = tabs[0];
                    if (tab) {
                        resolve(JSON.stringify({
                            id: tab.id,
                            url: tab.url,
                            title: tab.title,
                            active: tab.active,
                            windowId: tab.windowId
                        }));
                    } else {
                        resolve('');
                    }
                });
            });
        },

        queryTabs: function(jsonQuery) {
            return safeAsync(resolve => {
                const query = JSON.parse(jsonQuery);
                chrome.tabs.query(query, tabs => {
                    if (chrome.runtime.lastError) {
                        console.error('Tabs query error:', chrome.runtime.lastError);
                        resolve('[]');
                        return;
                    }
                    const simplifiedTabs = tabs.map(tab => ({
                        id: tab.id,
                        url: tab.url,
                        title: tab.title,
                        active: tab.active,
                        windowId: tab.windowId
                    }));
                    resolve(JSON.stringify(simplifiedTabs));
                });
            });
        },

        getTab: function(tabId) {
            return safeAsync(resolve => {
                chrome.tabs.get(tabId, tab => {
                    if (chrome.runtime.lastError) {
                        console.error('Get tab error:', chrome.runtime.lastError);
                        resolve('');
                        return;
                    }
                    resolve(JSON.stringify({
                        id: tab.id,
                        url: tab.url,
                        title: tab.title,
                        active: tab.active,
                        windowId: tab.windowId
                    }));
                });
            });
        },

        updateTab: function(tabId, jsonProperties) {
            return safeAsync(resolve => {
                const properties = JSON.parse(jsonProperties);
                chrome.tabs.update(tabId, properties, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Update tab error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        createTab: function(jsonProperties) {
            return safeAsync(resolve => {
                const properties = JSON.parse(jsonProperties);
                chrome.tabs.create(properties, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Create tab error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        }
    };

    // Action APIs
    const action = {
        setBadgeText: function(tabId, text) {
            return safeAsync(resolve => {
                chrome.action.setBadgeText({ tabId: tabId, text: text }, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Set badge text error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        setBadgeBackgroundColor: function(tabId, color) {
            return safeAsync(resolve => {
                chrome.action.setBadgeBackgroundColor({ tabId: tabId, color: color }, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Set badge color error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        setActionTitle: function(title) {
            return safeAsync(resolve => {
                chrome.action.setTitle({ title: title }, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Set action title error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        },

        setActionIcon: function(jsonIconData) {
            return safeAsync(resolve => {
                const iconData = JSON.parse(jsonIconData);
                chrome.action.setIcon(iconData, () => {
                    if (chrome.runtime.lastError) {
                        console.error('Set action icon error:', chrome.runtime.lastError);
                    }
                    resolve();
                });
            });
        }
    };

    // DOM and Page APIs (for content scripts)
    const dom = {
        getPageSource: function() {
            return document.documentElement.outerHTML;
        },

        querySelectors: function(selector) {
            try {
                const elements = document.querySelectorAll(selector);
                const result = Array.from(elements).map(el => ({
                    tagName: el.tagName,
                    id: el.id || null,
                    className: el.className || null,
                    innerText: el.innerText || '',
                    innerHTML: el.innerHTML || ''
                }));
                return JSON.stringify(result);
            } catch (error) {
                console.error('Query selectors error:', error);
                return '[]';
            }
        },

        querySelector: function(selector) {
            try {
                const element = document.querySelector(selector);
                if (element) {
                    return JSON.stringify({
                        tagName: element.tagName,
                        id: element.id || null,
                        className: element.className || null,
                        innerText: element.innerText || '',
                        innerHTML: element.innerHTML || ''
                    });
                }
                return '';
            } catch (error) {
                console.error('Query selector error:', error);
                return '';
            }
        },

        setElementText: function(selector, text) {
            try {
                const element = document.querySelector(selector);
                if (element) {
                    element.innerText = text;
                }
            } catch (error) {
                console.error('Set element text error:', error);
            }
        },

        setElementHtml: function(selector, html) {
            try {
                const element = document.querySelector(selector);
                if (element) {
                    element.innerHTML = html;
                }
            } catch (error) {
                console.error('Set element HTML error:', error);
            }
        }
    };

    // Fetch API with timeout
    const network = {
        fetch: async function(url, jsonOptions) {
            try {
                const options = JSON.parse(jsonOptions || '{}');
                const controller = new AbortController();
                const timeoutId = setTimeout(() => controller.abort(), options.timeoutMs || 5000);

                const fetchOptions = {
                    method: options.method || 'GET',
                    headers: options.headers || {},
                    body: options.body || undefined,
                    signal: controller.signal
                };

                const response = await fetch(url, fetchOptions);
                clearTimeout(timeoutId);

                const headers = {};
                response.headers.forEach((value, key) => {
                    headers[key] = value;
                });

                const body = await response.text();

                return JSON.stringify({
                    status: response.status,
                    statusText: response.statusText,
                    headers: headers,
                    body: body,
                    ok: response.ok
                });
            } catch (error) {
                return JSON.stringify({
                    status: 0,
                    statusText: error.message,
                    headers: {},
                    body: '',
                    ok: false
                });
            }
        }
    };

    // Public API
    return {
        // Storage
        getStorageLocal: storage.getStorageLocal,
        getStorageLocalMultiple: storage.getStorageLocalMultiple,
        setStorageLocal: storage.setStorageLocal,
        setStorageLocalMultiple: storage.setStorageLocalMultiple,
        removeStorageLocal: storage.removeStorageLocal,
        removeStorageLocalMultiple: storage.removeStorageLocalMultiple,
        clearStorageLocal: storage.clearStorageLocal,
        getStorageSession: storage.getStorageSession,
        setStorageSession: storage.setStorageSession,
        removeStorageSession: storage.removeStorageSession,
        getStorageManaged: storage.getStorageManaged,

        // Runtime
        sendMessage: runtime.sendMessage,
        sendMessageToTab: runtime.sendMessageToTab,
        getExtensionId: runtime.getExtensionId,
        getUrl: runtime.getUrl,

        // Tabs
        getActiveTab: tabs.getActiveTab,
        queryTabs: tabs.queryTabs,
        getTab: tabs.getTab,
        updateTab: tabs.updateTab,
        createTab: tabs.createTab,

        // Action
        setBadgeText: action.setBadgeText,
        setBadgeBackgroundColor: action.setBadgeBackgroundColor,
        setActionTitle: action.setActionTitle,
        setActionIcon: action.setActionIcon,

        // DOM
        getPageSource: dom.getPageSource,
        querySelectors: dom.querySelectors,
        querySelector: dom.querySelector,
        setElementText: dom.setElementText,
        setElementHtml: dom.setElementHtml,

        // Network
        fetch: network.fetch,

        // Cleanup
        dispose: function() {
            console.log('Chrome interop disposed');
        }
    };
})();
// PWA Install and Service Worker Management
let deferredPrompt;
let isInstalled = false;

// Check if app is already installed
window.addEventListener('beforeinstallprompt', (e) => {
    console.log('PWA: beforeinstallprompt event fired');
    // Prevent Chrome 67 and earlier from automatically showing the prompt
    e.preventDefault();
    // Stash the event so it can be triggered later
    deferredPrompt = e;
    // Show install button or notification
    showInstallButton();
});

// Handle successful installation
window.addEventListener('appinstalled', (evt) => {
    console.log('PWA: App was installed');
    isInstalled = true;
    hideInstallButton();
    // Show success message or redirect
    showInstallSuccess();
});

// Check if running as installed PWA
function isRunningAsPWA() {
    return window.matchMedia('(display-mode: standalone)').matches || 
           window.navigator.standalone === true;
}

// Show install button
function showInstallButton() {
    if (isRunningAsPWA()) return;
    
    // Create install button if it doesn't exist
    let installBtn = document.getElementById('pwa-install-btn');
    if (!installBtn) {
        installBtn = document.createElement('button');
        installBtn.id = 'pwa-install-btn';
        installBtn.innerText = '📱 Install App';
        installBtn.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 12px 20px;
            border-radius: 25px;
            cursor: pointer;
            font-weight: bold;
            box-shadow: 0 4px 15px rgba(0,0,0,0.2);
            z-index: 1000;
            transition: all 0.3s ease;
        `;
        
        installBtn.addEventListener('mouseover', () => {
            installBtn.style.transform = 'translateY(-2px)';
            installBtn.style.boxShadow = '0 6px 20px rgba(0,0,0,0.3)';
        });
        
        installBtn.addEventListener('mouseout', () => {
            installBtn.style.transform = 'translateY(0)';
            installBtn.style.boxShadow = '0 4px 15px rgba(0,0,0,0.2)';
        });
        
        installBtn.addEventListener('click', installPWA);
        document.body.appendChild(installBtn);
    }
    installBtn.style.display = 'block';
}

// Hide install button
function hideInstallButton() {
    const installBtn = document.getElementById('pwa-install-btn');
    if (installBtn) {
        installBtn.style.display = 'none';
    }
}

// Install PWA
async function installPWA() {
    if (!deferredPrompt) {
        console.log('PWA: No deferred prompt available');
        return;
    }
    
    // Show the install prompt
    deferredPrompt.prompt();
    
    // Wait for the user to respond to the prompt
    const { outcome } = await deferredPrompt.userChoice;
    console.log(`PWA: User choice: ${outcome}`);
    
    if (outcome === 'accepted') {
        console.log('PWA: User accepted the install prompt');
    } else {
        console.log('PWA: User dismissed the install prompt');
    }
    
    // Clear the deferred prompt
    deferredPrompt = null;
    hideInstallButton();
}

// Show install success message
function showInstallSuccess() {
    // Create success notification
    const successMsg = document.createElement('div');
    successMsg.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #4CAF50;
        color: white;
        padding: 15px 20px;
        border-radius: 5px;
        z-index: 1001;
        font-weight: bold;
        box-shadow: 0 4px 15px rgba(0,0,0,0.2);
    `;
    successMsg.innerText = '✅ App installed successfully!';
    document.body.appendChild(successMsg);
    
    // Remove after 3 seconds
    setTimeout(() => {
        document.body.removeChild(successMsg);
    }, 3000);
}

// Enhanced Service Worker Registration
if ('serviceWorker' in navigator) {
    window.addEventListener('load', async () => {
        try {
            const registration = await navigator.serviceWorker.register('service-worker.js');
            console.log('PWA: Service Worker registered successfully:', registration);
            
            // Handle updates
            registration.addEventListener('updatefound', () => {
                const newWorker = registration.installing;
                newWorker.addEventListener('statechange', () => {
                    if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
                        // New content is available, show update notification
                        showUpdateNotification();
                    }
                });
            });
            
        } catch (error) {
            console.error('PWA: Service Worker registration failed:', error);
        }
    });
}

// Show update notification
function showUpdateNotification() {
    const updateMsg = document.createElement('div');
    updateMsg.style.cssText = `
        position: fixed;
        top: 20px;
        left: 50%;
        transform: translateX(-50%);
        background: #2196F3;
        color: white;
        padding: 15px 20px;
        border-radius: 5px;
        z-index: 1001;
        font-weight: bold;
        box-shadow: 0 4px 15px rgba(0,0,0,0.2);
        cursor: pointer;
    `;
    updateMsg.innerHTML = '🔄 New version available! Click to update';
    
    updateMsg.addEventListener('click', () => {
        window.location.reload();
    });
    
    document.body.appendChild(updateMsg);
    
    // Auto-remove after 10 seconds
    setTimeout(() => {
        if (document.body.contains(updateMsg)) {
            document.body.removeChild(updateMsg);
        }
    }, 10000);
}

// Initialize PWA features when DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Check if already running as PWA
    if (isRunningAsPWA()) {
        console.log('PWA: Running as installed PWA');
        isInstalled = true;
    } else {
        console.log('PWA: Running in browser');
        // Show install prompt after a delay
        setTimeout(showInstallButton, 3000);
    }
    
    // Initialize offline handling
    initOfflineHandling();
});

// Offline/Online handling
function initOfflineHandling() {
    // Check initial connection status
    updateConnectivityStatus();
    
    // Listen for connection changes
    window.addEventListener('online', () => {
        console.log('PWA: Back online');
        updateConnectivityStatus();
        hideOfflineIndicator();
    });
    
    window.addEventListener('offline', () => {
        console.log('PWA: Gone offline');
        updateConnectivityStatus();
        showOfflineIndicator();
    });
}

function updateConnectivityStatus() {
    const isOnline = navigator.onLine;
    document.body.classList.toggle('offline', !isOnline);
    document.body.classList.toggle('online', isOnline);
}

function showOfflineIndicator() {
    let indicator = document.getElementById('offline-indicator');
    if (!indicator) {
        indicator = document.createElement('div');
        indicator.id = 'offline-indicator';
        indicator.className = 'offline-indicator';
        indicator.innerHTML = '⚠️ You are offline. Some features may not work.';
        document.body.appendChild(indicator);
    }
    indicator.classList.add('show');
}

function hideOfflineIndicator() {
    const indicator = document.getElementById('offline-indicator');
    if (indicator) {
        indicator.classList.remove('show');
        setTimeout(() => {
            if (document.body.contains(indicator)) {
                document.body.removeChild(indicator);
            }
        }, 300);
    }
}

// Export functions for potential use by Blazor
window.PWAHelper = {
    isInstalled: () => isInstalled,
    isRunningAsPWA: isRunningAsPWA,
    installPWA: installPWA,
    showInstallButton: showInstallButton,
    hideInstallButton: hideInstallButton,
    isOnline: () => navigator.onLine
};

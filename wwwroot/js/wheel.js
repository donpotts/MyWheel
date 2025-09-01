let currentWheelRotation = 0;
let wheelAnimationStart = 0;
let wheelAnimationDuration = 0;
let wheelStartRotation = 0;
let wheelTargetRotation = 0;
let wheelAnimationId = null;

// Initialize wheel rotation tracking
window.initWheelRotation = (initialRotation = 0) => {
    currentWheelRotation = initialRotation;
};

window.setWheelRotation = (rotation, duration) => {
    const wheel = document.getElementById('wheel');
    if (wheel) {
        // Store animation parameters
        wheelStartRotation = currentWheelRotation;
        wheelTargetRotation = rotation;
        wheelAnimationDuration = duration;
        wheelAnimationStart = Date.now();
        
        // Set CSS properties
        wheel.style.setProperty('--start-rotation', wheelStartRotation + 'deg');
        wheel.style.setProperty('--spin-rotation', rotation + 'deg');
        wheel.style.setProperty('--spin-duration', duration + 'ms');
        
        // Start tracking rotation with requestAnimationFrame
        startWheelAnimation();
        
        // Initialize audio and start ticking sound
        initAudioContext();
        startTickingSound(duration);
    }
};

function startWheelAnimation() {
    function updateRotation() {
        const elapsed = Date.now() - wheelAnimationStart;
        const progress = Math.min(elapsed / wheelAnimationDuration, 1);
        
        // Use realistic deceleration curve for weighted wheel effect
        // cubic-bezier(0.17, 0.67, 0.12, 0.99) - starts fast, slows down naturally
        const easedProgress = cubicBezier(progress, 0.17, 0.67, 0.12, 0.99);
        
        currentWheelRotation = wheelStartRotation + (wheelTargetRotation - wheelStartRotation) * easedProgress;
        
        if (progress < 1) {
            wheelAnimationId = requestAnimationFrame(updateRotation);
        } else {
            currentWheelRotation = wheelTargetRotation;
            wheelAnimationId = null;
        }
    }
    
    if (wheelAnimationId) {
        cancelAnimationFrame(wheelAnimationId);
    }
    
    wheelAnimationId = requestAnimationFrame(updateRotation);
}

// Improved cubic bezier implementation for realistic wheel deceleration
function cubicBezier(t, x1, y1, x2, y2) {
    // More accurate cubic bezier calculation
    // This creates a realistic deceleration curve like a real weighted wheel
    const cx = 3 * x1;
    const bx = 3 * (x2 - x1) - cx;
    const ax = 1 - cx - bx;
    
    const cy = 3 * y1;
    const by = 3 * (y2 - y1) - cy;
    const ay = 1 - cy - by;
    
    return ((ax * t + bx) * t + cx) * t + ay * t * t * t + by * t * t + cy * t;
}

// Function to stop wheel spinning
window.stopWheelSpin = () => {
    const wheel = document.getElementById('wheel');
    if (wheel) {
        // Cancel the animation frame
        if (wheelAnimationId) {
            cancelAnimationFrame(wheelAnimationId);
            wheelAnimationId = null;
        }
        
        // Use the tracked rotation
        let stoppedRotation = currentWheelRotation % 360;
        if (stoppedRotation < 0) {
            stoppedRotation += 360;
        }
        
        // Remove the spinning class and set final position
        wheel.classList.remove('spinning');
        wheel.style.transform = `rotate(${stoppedRotation}deg)`;
        wheel.style.setProperty('--spin-duration', '0ms');
        wheel.style.setProperty('--spin-rotation', stoppedRotation + 'deg');
        wheel.style.animation = 'none';
        
        // Update our tracked rotation
        currentWheelRotation = stoppedRotation;
        
        // Stop ticking sound
        stopTicking();
        
        console.log('Wheel stopped at rotation:', stoppedRotation);
        
        return stoppedRotation;
    }
    return 0;
};

let tickingInterval;
let audioContext;
let isAudioInitialized = false;

// Initialize audio context once
function initAudioContext() {
    if (!isAudioInitialized) {
        try {
            audioContext = new (window.AudioContext || window.webkitAudioContext)();
            isAudioInitialized = true;
        } catch (error) {
            console.log('Audio not supported');
        }
    }
    
    // Resume audio context if suspended
    if (audioContext && audioContext.state === 'suspended') {
        audioContext.resume();
    }
}

function startTickingSound(duration) {
    // Clear any existing ticking
    stopTicking();
    
    if (!audioContext) return;
    
    // Ticking should slow down as the wheel decelerates, like a real wheel
    // Stop ticking 800ms before animation ends for realistic effect
    const tickingDuration = duration - 800;
    let startTime = Date.now();
    let lastTickTime = 0;
    
    function scheduleNextTick() {
        const elapsed = Date.now() - startTime;
        const progress = elapsed / tickingDuration;
        
        if (elapsed >= tickingDuration || progress >= 1) {
            stopTicking();
            return;
        }
        
        // Calculate dynamic tick rate - starts fast, slows down
        // Early: 80ms between ticks, Late: 300ms between ticks
        const baseTickRate = 80;
        const maxTickRate = 300;
        const tickRate = baseTickRate + (maxTickRate - baseTickRate) * (progress * progress);
        
        const now = Date.now();
        if (now - lastTickTime >= tickRate) {
            playTickSound();
            lastTickTime = now;
        }
        
        // Schedule next check
        tickingInterval = setTimeout(scheduleNextTick, 20);
    }
    
    scheduleNextTick();
}

function stopTicking() {
    if (tickingInterval) {
        clearTimeout(tickingInterval);
        tickingInterval = null;
    }
}

// Expose globally for Blazor to call
window.stopTicking = stopTicking;

function playTickSound() {
    if (!audioContext) return;
    
    try {
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();
        
        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);
        
        // Short tick sound
        oscillator.frequency.setValueAtTime(1000, audioContext.currentTime);
        oscillator.type = 'square';
        
        gainNode.gain.setValueAtTime(0, audioContext.currentTime);
        gainNode.gain.linearRampToValueAtTime(0.05, audioContext.currentTime + 0.001);
        gainNode.gain.exponentialRampToValueAtTime(0.001, audioContext.currentTime + 0.03);
        
        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.03);
        
    } catch (error) {
        // Ignore audio errors
    }
}

window.playSound = (soundType) => {
    // Initialize audio context if needed
    initAudioContext();
    
    if (!audioContext) return;
    
    try {
        // Generate different tones for different sound types
        let frequency, duration, type;
        
        switch (soundType) {
            case 'spin':
                // Initial spin sound - quick whoosh
                frequency = 300;
                duration = 0.2;
                type = 'sawtooth';
                break;
            case 'select':
                // Selection sound - ding
                frequency = 800;
                duration = 0.3;
                type = 'sine';
                break;
            case 'confirm':
                // Confirmation sound - success chime
                frequency = 600;
                duration = 0.4;
                type = 'triangle';
                break;
            default:
                return;
        }
        
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();
        
        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);
        
        oscillator.frequency.setValueAtTime(frequency, audioContext.currentTime);
        oscillator.type = type;
        
        // Envelope for smooth sound
        gainNode.gain.setValueAtTime(0, audioContext.currentTime);
        gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 0.01);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + duration);
        
        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + duration);
        
        // Special effect for spin sound - frequency sweep
        if (soundType === 'spin') {
            oscillator.frequency.linearRampToValueAtTime(frequency * 1.5, audioContext.currentTime + duration);
        }
        
        // Special effect for confirm sound - double tone
        if (soundType === 'confirm') {
            setTimeout(() => {
                if (!audioContext) return;
                
                const oscillator2 = audioContext.createOscillator();
                const gainNode2 = audioContext.createGain();
                
                oscillator2.connect(gainNode2);
                gainNode2.connect(audioContext.destination);
                
                oscillator2.frequency.setValueAtTime(frequency * 1.5, audioContext.currentTime);
                oscillator2.type = 'sine';
                
                gainNode2.gain.setValueAtTime(0, audioContext.currentTime);
                gainNode2.gain.linearRampToValueAtTime(0.2, audioContext.currentTime + 0.01);
                gainNode2.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.2);
                
                oscillator2.start(audioContext.currentTime);
                oscillator2.stop(audioContext.currentTime + 0.2);
            }, 150);
        }
        
    } catch (error) {
        console.log('Audio playback failed:', error);
    }
};

// Initialize audio context on first user interaction
document.addEventListener('click', function initAudio() {
    try {
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        if (audioContext.state === 'suspended') {
            audioContext.resume();
        }
    } catch (e) {
        console.log('Audio context initialization failed');
    }
    document.removeEventListener('click', initAudio);
}, { once: true });

// Function to download JSON configuration
window.downloadJson = (filename, jsonContent) => {
    const blob = new Blob([jsonContent], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

// Function to download text files (CSV, etc.)
window.downloadText = (filename, textContent, mimeType = 'text/plain') => {
    const blob = new Blob([textContent], { type: mimeType });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
};

// Function to click file input
window.clickFileInput = (element) => {
    if (element && typeof element.click === 'function') {
        element.click();
    }
};

// Function to get selected files
window.getSelectedFiles = (element) => {
    if (element.files && element.files.length > 0) {
        return Array.from(element.files).map(file => file.name);
    }
    return [];
};

// Function to read file as text
window.readFileAsText = (element) => {
    return new Promise((resolve, reject) => {
        if (element.files && element.files.length > 0) {
            const file = element.files[0];
            const reader = new FileReader();
            reader.onload = (e) => resolve(e.target.result);
            reader.onerror = (e) => reject(e);
            reader.readAsText(file);
        } else {
            reject('No file selected');
        }
    });
};

// Fallback function for copying to clipboard in older browsers
window.copyToClipboardFallback = (text) => {
    const textArea = document.createElement('textarea');
    textArea.value = text;
    textArea.style.position = 'fixed';
    textArea.style.left = '-999999px';
    textArea.style.top = '-999999px';
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    
    try {
        document.execCommand('copy');
    } catch (err) {
        console.error('Fallback copy failed:', err);
    }
    
    document.body.removeChild(textArea);
};

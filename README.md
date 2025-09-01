# 🎡 My Wheel

[![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=for-the-badge&logo=blazor&logoColor=white)](https://blazor.net)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com)
[![WebAssembly](https://img.shields.io/badge/WebAssembly-654FF0?style=for-the-badge&logo=webassembly&logoColor=white)](https://webassembly.org)

> 🌟 A beautiful, interactive spinning wheel app that makes recipe selection fun and exciting! Built with modern Blazor WebAssembly technology.

**🌐 Live Demo: [https://donpotts.github.io/MyWheel/](https://donpotts.github.io/MyWheel/)**

## ✨ Features

🎨 **Beautiful Animations** - Realistic weighted wheel physics with 5-second deceleration  
🔊 **Dynamic Sound Effects** - Progressive ticking that slows down naturally with the wheel  
📱 **Responsive Design** - Perfect experience on desktop, tablet, and mobile  
🎯 **Smart Selection** - Intelligent wheel physics with momentum and realistic stopping  
🛑 **Interactive Stop Button** - Click the center circle to stop the wheel exactly where you want  
🌈 **Modern UI** - Stunning gradients, emojis, and hover effects  
🍽️ **8 Delicious Recipes** - Curated selection of amazing dishes to try  
🤖 **AI Recipe Generation** - Get detailed recipes with daily access codes

## 🎡 New Interactive Features

### � Smart Stop Button
- **Red Center Circle**: Appears when wheel is spinning
- **Precise Control**: Stop exactly where the indicator points
- **Visual Feedback**: Glowing red effect with hover animations
- **Real-time Tracking**: JavaScript monitors exact wheel position
- **Cancellation System**: Properly stops timers and audio

### ⚡ Realistic Wheel Physics
- **5-Second Duration**: Extended timing for better user experience
- **Weighted Deceleration**: Starts fast, naturally slows down like a real wheel
- **Dynamic Audio**: Ticking starts rapid (80ms) and slows to (300ms intervals)
- **More Rotations**: 6-11 full spins for dramatic effect
- **Cubic Bezier Easing**: `cubic-bezier(0.17, 0.67, 0.12, 0.99)` for authentic feel

### 🎨 Enhanced User Experience
- **🎯 Emoji Integration**: Beautiful emojis replace placeholder text throughout
- **🔐 Proactive Access**: Recipe unlock prompt shows immediately when needed
- **🛑 Stop Message**: Special "Stopped!" message when manually stopped
- **⚡ Improved Feedback**: Better visual and audio cues for all interactions

## 🔐 Daily Recipe Access System

The wheel features a secure daily access control system **for AI recipe generation only**:

- **🆓 Free Wheel Access**: Anyone can spin the wheel and select items freely
- **🗓️ Daily Recipe Codes**: Unique codes required only for AI-generated recipes
- **⏰ Time-Limited**: 24-hour access for recipe generation from midnight to midnight
- **🛡️ Secure**: Cryptographically generated codes prevent unauthorized recipe access
- **📱 Device-Specific**: One recipe code per device per day

### For Users
1. **🎡 Spin the wheel freely** - No code needed for basic wheel functionality
2. **🛑 Stop anytime** - Click the red center circle to stop the wheel exactly where you want
3. **🔐 Unlock recipes upfront** - See the recipe access prompt immediately if codes are needed
4. **🍽️ Generate detailed recipes** - Enter today's access code to unlock AI-powered recipe generation
5. **⏱️ Enjoy realistic physics** - 5-second weighted wheel with natural deceleration

## 🚀 Quick Start

### Prerequisites
- 🔧 [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- 💻 Any modern web browser

### 🏃‍♂️ Running the App
```bash
# Clone the repository
git clone https://github.com/donpotts/MyWheel.git
cd MyWheel

# Run the application
dotnet run --project MyWheelApp.csproj
```

🌐 Open your browser and navigate to `http://localhost:5234`

### 🎮 How to Play
1. 🖱️ Click the **"Spin Wheel"** button to start the magic
2. ⏳ Watch the wheel spin with realistic weighted physics (5-second deceleration)
3. 🛑 **Optional**: Click the red center circle to stop exactly where you want
4. 🎉 Wait for it to land on your destiny recipe (auto-selected!)
5. 🔐 **If needed**: Unlock recipe access with daily code (prompt shown immediately)
6. 🍽️ **Optional**: Click **"Get Recipe"** for a detailed AI-generated recipe
7. 👩‍🍳 Start cooking your randomly selected masterpiece!

## 🏗️ Project Architecture

```
📁 MyWheel/
├── 📄 Pages/Home.razor           # 🎡 Main wheel component
├── 📄 Pages/Configuration.razor  # ⚙️ Wheel customization
├── 📄 Pages/AccessCode.razor     # 🔐 Daily code entry (for recipes only)
├── 📄 Pages/AdminCodes.razor     # 👨‍💼 Admin code management
├── 📁 Services/
│   ├── 🛠️ WheelConfigurationService.cs
│   ├── 🔐 DailyCodeService.cs    # Recipe access control logic
│   ├── 🤖 RecipeService.cs       # AI recipe generation
│   └── 📄 IRecipeService.cs      # Recipe service interface
├── 📁 Models/
│   ├── 🎯 WheelConfiguration.cs
│   ├── 🔐 DailyCode.cs          # Access control models
│   └── 🍽️ Recipe.cs              # Recipe data model
├── 📁 wwwroot/
│   ├── 🎨 css/app.css           # Stunning animations & styles
│   ├── ⚡ js/wheel.js            # Wheel physics & sound magic
│   ├── 🌐 index.html            # Entry point
│   └── 📱 manifest.json         # PWA configuration
└── 📱 Layout/                   # App shell & navigation
```

## 🍽️ Recipe Collection

| 🎯 Slot | 🥘 Recipe | 🌟 Difficulty |
|---------|-----------|---------------|
| 1 | 🍕 Pizza Margherita | ⭐⭐ |
| 2 | 🍛 Chicken Curry | ⭐⭐⭐ |
| 3 | 🌮 Beef Tacos | ⭐⭐ |
| 4 | 🍝 Pasta Carbonara | ⭐⭐ |
| 5 | 🐟 Salmon Teriyaki | ⭐⭐⭐ |
| 6 | 🥬 Vegetable Stir Fry | ⭐ |
| 7 | 🥗 Greek Salad | ⭐ |
| 8 | 🍰 Chocolate Cake | ⭐⭐⭐⭐ |

## 🛠️ Tech Stack

| Technology | Purpose | Version |
|------------|---------|---------|
| ![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=flat&logo=blazor&logoColor=white) | Frontend Framework | .NET 9 |
| ![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white) | Backend Logic | C# 13.0 |
| ![CSS3](https://img.shields.io/badge/CSS3-1572B6?style=flat&logo=css3&logoColor=white) | Physics Animations | CSS3 + Cubic Bezier |
| ![JavaScript](https://img.shields.io/badge/JavaScript-ES6+-F7DF1E?style=flat&logo=javascript&logoColor=black) | Real-time Tracking | Web Audio API + RequestAnimationFrame |
| ![WebAssembly](https://img.shields.io/badge/WebAssembly-654FF0?style=flat&logo=webassembly&logoColor=white) | Runtime | Latest |
| ![Semantic Kernel](https://img.shields.io/badge/Semantic_Kernel-0078D4?style=flat&logo=microsoft&logoColor=white) | AI Integration | Latest |

## ⚙️ Technical Improvements

### 🎯 Real-time Wheel Tracking
- **RequestAnimationFrame**: Precise rotation tracking during animation
- **Cancellation Tokens**: Proper async operation management
- **CSS-JS Sync**: Perfect synchronization between visual and logic layers
- **Memory Management**: IDisposable implementation for cleanup

### 🔊 Advanced Audio System
- **Web Audio API**: Programmatic sound generation
- **Dynamic Timing**: Progressive tick rates based on wheel speed
- **Audio Context**: Proper initialization and state management
- **Cross-browser**: Fallback support for older browsers

## 🚀 Deployment

### GitHub Pages
The app is automatically deployed to GitHub Pages. The daily code system works seamlessly in production for recipe access.

### Local Development
1. Clone the repository
2. Run `dotnet run` from the solution directory
3. Wheel works immediately

## 📧 Contact for Recipe Codes or Support

**Don Potts**  
📮 Email: [Don.Potts@DonPotts.com](mailto:Don.Potts@DonPotts.com)

---

<div align="center">
  
**Made with ❤️ using Blazor WebAssembly**

*Spin freely, cook with access! 🎡🍽️*

</div>


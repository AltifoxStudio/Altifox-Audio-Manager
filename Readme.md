# Altifox Audio Manager
[![License: CC BY-NC 4.0](https://img.shields.io/badge/License-CC%20BY--NC%204.0-lightgrey.svg)](https://creativecommons.org/licenses/by-nc/4.0/)

Altifox Audio Manager is a Unity package created to bring dynamic audio features, inspired by tools like **Wwise** and **FMOD**, directly into the Unity editor. It was specifically designed to offer these advanced capabilities with better support for **WebGL** platforms.

---

## ✨ Features

* **Dynamic Music Layering:** Easily set up and control multiple music layers with built-in automation for volume and pitch.
* **OneShot Player:** A simple component to add sound effects (`SFX`) to any `GameObject`. It includes a drag-and-drop interface and lets you trigger sounds on standard Unity events (e.g., `OnEnable`, `OnTriggerEnter`).
* **AudioSource Pooling:** An efficient pooling system for `AudioSource` components to minimize performance overhead. It features two modes:
    * **Single-Buffered:** A lightweight option for simple, non-looping sounds.
    * **Double-Buffered:** Perfect for dynamic looping and creating seamless audio transitions.

---

## ⚙️ Installation

1.  Open your Unity project.
2.  Navigate to `Window` > `Package Manager`.
3.  Click the `+` icon in the top-left corner and select `Add package from git URL...`.
4.  Paste the repository's git URL and click `Add`.

Alternatively, you can download the package and add it from your local disk by choosing `Add package from disk...`.

---

## 🗺️ Roadmap

Here are the features planned for future updates:

* **Playlist Crossfading:** Implement smooth crossfades between different audio clips within a playlist.
* **Crossfade Looping:** Create seamlessly looping audio with crossfades at the loop points.
* **Soundscape Component:** Develop a new `MonoBehaviour` for easily building and managing complex ambient sound environments.

---

## 📜 License

This project is licensed under the **Creative Commons Attribution-NonCommercial 4.0 International License**. Please see the `LICENSE.md` file for full details.
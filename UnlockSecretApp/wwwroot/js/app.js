import { initSwipe } from "./swipe.js";
import { initPages } from "./pages.js";
import { initGallery } from "./gallery.js";

window.addEventListener("DOMContentLoaded", () => {

    // Проверка кода
    const correctCode = ["20", "05", "10"];
    initSwipe(correctCode);

    // Логика страниц
    initPages();

    // Данные галереи
    const galleryData = [
        { image: "/images/1.jpg", caption: "Наша первая встреча 💫" },
        { image: "/images/2.jpg", caption: "Тот день, когда я понял — ты особенная ✨" },
        { image: "/images/3.jpg", caption: "Моменты счастья 🌸" },
        { image: "/images/4.jpg", caption: "Тепло, которое ты даришь 💖" },
        { image: "/images/5.jpg", caption: "Наши приключения 😊" },
        { image: "/images/6.jpg", caption: "Тихие вечера 🌙" }
    ];

    initGallery(galleryData);

    // Auto-focus
    document.querySelector(".code").focus();
});

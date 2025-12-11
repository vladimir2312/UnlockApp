import { createParticles, createWave } from "./particles.js";
import { showPage } from "./helpers.js";

export function initSwipe(correctCode) {
    const kitten = $("#kitten");
    const swipeBar = $(".swipe-bar");
    const kittenInner = $(".kitten-inner");
    const message = $("#message");

    let startX = 0;
    let isSwiping = false;

    kitten.addEventListener("pointerdown", e => {
        startX = e.clientX;
        isSwiping = true;

        createWave(e.clientX, e.clientY);
    });

    kitten.addEventListener("pointermove", e => {
        if (!isSwiping) return;

        let dx = e.clientX - startX;
        if (dx < 0) dx = 0;

        const progress = Math.min(dx / 150 * 100, 100);

        swipeBar.style.width = progress + "%";
        kittenInner.style.left = (dx / 2) + "px";
    });

    kitten.addEventListener("pointerup", () => {
        if (!isSwiping) return;
        isSwiping = false;

        const dx = kittenInner.offsetLeft;

        if (dx > 80) {
            checkCode();
        } else {
            resetSwipe();
        }
    });

    function resetSwipe() {
        swipeBar.style.width = "0%";
        kittenInner.style.left = "10px";
    }

    function checkCode() {
        const inputs = $$(".code");
        const userCode = [...inputs].map(i => i.value.padStart(2, "0"));

        if (userCode.join(",") !== correctCode.join(",")) {
            message.textContent = "Неверный код! Попробуй ещё 🌸";
            resetSwipe();
            return;
        }

        message.textContent = "Правильно! 🎉";
        showPage("page1");
    }
}

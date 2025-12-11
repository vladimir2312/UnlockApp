export function createParticles(x, y, count = 5) {
    for (let i = 0; i < count; i++) {
        const p = document.createElement("div");
        p.className = "particle";

        p.style.left = x + "px";
        p.style.top = y + "px";

        const tx = (Math.random() - 0.5) * 60;
        const ty = (Math.random() - 0.5) * 60;

        p.style.setProperty("--tx", tx + "px");
        p.style.setProperty("--ty", ty + "px");

        document.body.appendChild(p);
        setTimeout(() => p.remove(), 800);
    }
}

export function createWave(x, y) {
    const w = document.createElement("div");
    w.className = "wave";

    w.style.left = x + "px";
    w.style.top = y + "px";

    document.body.appendChild(w);
    setTimeout(() => w.remove(), 400);
}

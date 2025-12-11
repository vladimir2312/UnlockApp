// Упрощённый выбор элементов
const $ = (sel) => document.querySelector(sel);
const $$ = (sel) => document.querySelectorAll(sel);

// Плавное появление страницы
export function showPage(pageId) {
    const pages = ["page1", "page2", "page3", "page4"];

    pages.forEach(id => {
        let el = document.getElementById(id);
        if (id === pageId) {
            el.classList.remove("hidden");
            setTimeout(() => el.classList.add("show"), 20);
        } else {
            el.classList.remove("show");
            setTimeout(() => el.classList.add("hidden"), 300);
        }
    });
}

export function delay(ms) {
    return new Promise(res => setTimeout(res, ms));
}

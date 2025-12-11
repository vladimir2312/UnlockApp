import { showPage, delay } from "./helpers.js";
import { typeWriter } from "./typing.js";
import { initGallery } from "./gallery.js";

export async function initPages() {

    // PAGE 1
    $("#btnToPage2").onclick = () => showPage("page2");

    // PAGE 2
    $("#btnToPage3").onclick = () => showPage("page3");

    // PAGE 3
    $("#btnToPage4").onclick = async () => {
        await delay(200);
        showPage("page4");
    };

    // PAGE 4
    $("#btnFinish").onclick = () => {
        alert("Спасибо, что прошла весь путь со мной 💖");
    };
}


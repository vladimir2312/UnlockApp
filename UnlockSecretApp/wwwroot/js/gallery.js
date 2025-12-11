let index = 0;

export function initGallery(data) {
    const img = $("#galleryImage");
    const cap = $("#galleryCaption");

    const total = data.length;
    $("#totalImages").textContent = total;

    function show(i) {
        index = (i + total) % total;

        img.src = data[index].image;
        cap.textContent = data[index].caption;

        $("#currentIndex").textContent = index + 1;
    }

    $(".gallery-arrow-prev").onclick = () => show(index - 1);
    $(".gallery-arrow-next").onclick = () => show(index + 1);

    show(0);
}

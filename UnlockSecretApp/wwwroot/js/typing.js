export function typeWriter(text, element, speed = 35, done = null) {
    let i = 0;
    element.innerHTML = "";

    function type() {
        if (i >= text.length) {
            if (done) done();
            return;
        }

        element.innerHTML += text[i++];
        setTimeout(type, speed);
    }

    type();
}

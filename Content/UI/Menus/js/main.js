var styleModifier = document.head.appendChild(document.createElement("style"));
window.addEventListener("scroll", onScroll);
document.body.addEventListener("touchmove", onScroll);

function onScroll(e) {
    var scrollCutoff = 0.75;
    var heroInnerOpacity = roundToDp(1 - (window.pageYOffset / (window.innerHeight * scrollCutoff)), 2);
    styleModifier.innerHTML = `
        #hero::after {
            transform: translateY(${window.pageYOffset}px) rotate(45deg);
        }
        #hero * {
            opacity: ${heroInnerOpacity};
        }
    `;
}

function roundToDp(x, d) {
    var mult = Math.pow(10, d);
    return Math.round(x * mult) / mult;
}
var keybuffer = [];
var target = [38, 38, 40, 40, 37, 39, 37, 39, 66, 65, 13];
window.addEventListener("keydown", function (event) {
    keybuffer.push(event.keyCode);
    if (keybuffer.toString() == target.toString()) {
        document.getElementsByTagName("main")[0].innerHTML += `<iframe width="560" height="315" src="https://www.youtube.com/embed/hmJm373GtOg" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>`;
    }
    if (keybuffer.length > 10) keybuffer.splice(0, 1);
});
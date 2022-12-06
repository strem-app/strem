﻿function setZoom(newSize)
{
    document.body.style.zoom = `${newSize}%`;
    let scaleAlteration = ((100-newSize)/100);
    if(scaleAlteration >= 0) { scaleAlteration *= 1.75; }
    if(scaleAlteration < 0) { scaleAlteration /= 1.5; }

    console.log("alteration", scaleAlteration);
    const scalingFactor = 1+scaleAlteration;
    const base = 100;
    const newVhScaling = (100 * scalingFactor) + "vh";
    console.log("new scaling", newVhScaling);
    document.querySelectorAll(".is-fullheight").forEach(x => x.style.minHeight = newVhScaling);
}

function processButtonPicker()
{
    $('.is-icon-picker').iconpicker({
        placement: 'top',
        hideOnSelect: true,
        inputSearch: true
    })
}

function showNotification(text, type, duration)
{
    console.log("notifying", { text, type, duration });
    bulmaToast.toast({ message: text, type: type, duration: duration });
}

function animateElement(element, animation, prefix = 'animate__') {
    // We create a Promise and return it
    new Promise((resolve, reject) => {
        const animationName = `${prefix}${animation}`;
        element.classList.add(`${prefix}animated`, animationName);
        
        // When the animation ends, we clean the classes and resolve the Promise
        function handleAnimationEnd(event) {
            event.stopPropagation();
            element.classList.remove(`${prefix}animated`, animationName);
            resolve('Animation ended');
        }
        
        element.addEventListener('animationend', handleAnimationEnd, {once: true});
    });
}

function setNoSleepButton(element) {
    const noSleep = new NoSleep();
    let isStoppingSleep = false;
    element.addEventListener('click', function enableNoSleep() {
        if(isStoppingSleep) 
        { 
            noSleep.disable();
            element.innerText = "Anti-Sleep Mode Disabled";
        }
        else 
        {
            noSleep.enable();
            element.innerText = "Anti-Sleep Mode Enabled";
        }
        
        isStoppingSleep = !isStoppingSleep;
    }, false);
}

function setSliderColors(id, trackColor, thumbColor) {
    const element = document.getElementById(id);
    element.style.setProperty("--slider-track-color", trackColor, "");
    element.style.setProperty("--slider-thumb-color", thumbColor, "");
}

bulmaToast.setDefaults({
    duration: 2000,
    position: 'bottom-right',
    closeOnClick: true,
    animate: {in: 'bounceInRight', out: 'bounceOutRight'}
});
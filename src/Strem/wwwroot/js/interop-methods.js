function setZoom(newSize)
{
    document.body.style.zoom = `${newSize}%`;
    let scaleAlteration = ((100-newSize)/100);
    if(scaleAlteration >= 0) { scaleAlteration *= 1.5; }
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

function showNotification(text, type)
{
    bulmaToast.toast({ message: text, type: type })
}

bulmaToast.setDefaults({
    duration: 1000,
    position: 'bottom-right',
    closeOnClick: true
})
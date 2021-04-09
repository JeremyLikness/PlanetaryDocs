window.markdownExtensions = {
    toHtml: (txt, target) => {
        window.markdownExtensions.area =
            window.markdownExtensions.area ??
            document.createElement("textarea");
        setTimeout(() => {
            const area = window.markdownExtensions.area;
            area.innerHTML = txt;
            target.innerHTML = area.value;
        }, 0);
    }
}
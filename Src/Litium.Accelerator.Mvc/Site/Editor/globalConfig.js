CKEDITOR.on("instanceReady", function (ev) {
    ev.editor.dataProcessor.htmlFilter.addRules({
        elements: {
            $: function (element) {
                // add class for bulleted list
                if (element.name == 'ul') {
                    if (!element.attributes.class) {
                        element.attributes.class = "bulleted-list";
                    }
                }
                return element;
            }
        }
    });
});

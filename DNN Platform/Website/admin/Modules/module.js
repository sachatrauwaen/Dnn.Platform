$(function () {
    
    $('#dnnModuleSettings form').ajaxForm({
        success: function () {
            window.location = $('#dnnModuleSettings').attr('data-returnurl');
        },
        beforeSerialize: function () {
        }
    });
    
});

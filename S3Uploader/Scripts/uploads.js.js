(function() {
  jQuery(function() {
    var progressSupported, uploaderHost, xhrUploadProgressSupported;
    uploaderHost = "http://" + ($('#uploader').data('s3-bucket')) + ".s3.amazonaws.com";
    xhrUploadProgressSupported = function() {
      var xhr;
      xhr = new XMLHttpRequest();
      return xhr && ('upload' in xhr) && ('onprogress' in xhr.upload);
    };
    progressSupported = xhrUploadProgressSupported();
    $('#uploading_files').on('click', '.uploading_file .remove_link', function(e) {
      var uuid;
      uuid = $(this).parent().data('uuid');
      $(this).parent().remove();
      return $('#uploader iframe')[0].contentWindow.postMessage(JSON.stringify({
        eventType: 'abort upload',
        uuid: uuid
      }), uploaderHost);
    });
    return $(window).on("message", function(event) {
      var data, eventType, uploadPercent;
      event = event.originalEvent;
      if (event.origin !== uploaderHost) {
        return;
      }
      data = JSON.parse(event.data);
      eventType = data.eventType;
      delete data.eventType;
      switch (eventType) {
        case 'upload done':
          $(".uploading_file[data-uuid=" + data.uuid + "]").remove();
          return $.ajax($('#uploader iframe').data('create-resource-url'), {
            type: 'POST',
            data: data
          });
        case 'add upload':
          if (progressSupported) {
            uploadPercent = "<br/><progress value='0' max='100' class='upload_progress_bar'>0</progress> <span class='upload_percentage'>0</span> %";
            $('#uploading_files').append("<p class='uploading_file'>" + (data.file_name + uploadPercent) + " <a href='#' class='remove_link'>X</a></p>");
          } else {
            $('#uploading_files').append("<p class='uploading_file'>" + data.file_name + "<br/><img src='<%= asset_path('uploading.gif') %>'/></p>");
          }
          return $('.uploading_file').last().attr('data-uuid', data.uuid);
        case 'upload progress':
          if (progressSupported) {
            $(".uploading_file[data-uuid=" + data.uuid + "]").find('.upload_percentage').html(data.progress);
            return $(".uploading_file[data-uuid=" + data.uuid + "]").find('.upload_progress_bar').val(data.progress);
          }
      }
    });
  });
}).call(this);

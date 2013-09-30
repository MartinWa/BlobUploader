var MAX_BLOCK_SIZE = 256 * 1024;//Each file will be split in 256 KB.
var BLOCK_ID_PREFIX = "block-";
var RETRY_TIMEOUT_SECONDS = 5;
var NUMBER_OF_RETRIES = 3;

var selectedFile = null;

var currentFilePointer = 0;
var blockSize = 0;
var totalBytesRemaining = 0;
var blockIds = new Array();
var submitUri = null;
var bytesUploaded = 0;
var reader = new FileReader();

$(document).ready(function () {
    $("#output").hide();
    $("#progress").hide();
    $("#file").bind('change', handleFileSelect);
    $("#uploadFile").bind('click', startUpload);
    if (window.File && window.FileReader && window.FileList && window.Blob) {
        // Great success! All the File APIs are supported.
    } else {
        alert('The File APIs are not fully supported in this browser.');
    }
    var sasBase64 = $.url().param('sas');
    submitUri = atob(sasBase64);
});

reader.onloadend = function (evt) {
    if (evt.target.readyState == FileReader.DONE) { // DONE == 2
        var uri = submitUri + '&comp=block&blockid=' + blockIds[blockIds.length - 1];
        var requestData = new Uint8Array(evt.target.result);
        sendAjax(uri,
            requestData,
            function (xhr) {
                xhr.setRequestHeader('x-ms-blob-type', 'BlockBlob');
            },
            function (data, status) {
                console.log(status);
                bytesUploaded += requestData.length;
                var percentComplete = ((parseFloat(bytesUploaded) / parseFloat(selectedFile.size)) * 100).toFixed(2);
                $("#fileUploadProgress").text(percentComplete + " %");
                uploadFileInBlocks();
            });
    }
};

function sendAjax(url, dataToSend, beforeSendFunction, successFuction) {
    $.ajax({
        url: url,
        type: "PUT",
        data: dataToSend,
        processData: false,
        beforeSend: beforeSendFunction,
        tryCount: 0,
        retryLimit: NUMBER_OF_RETRIES,
        success: successFuction,
        error: function (xhr, desc, err) {
            if (xhr.status == 403) {
                $("#fileUploadProgress").text("Access denied, the assigned upload time has been exeeded");
            } else {
                this.tryCount++;
                if (this.tryCount <= this.retryLimit) {
                    console.log("Retrying transmission");
                    setTimeout($.ajax(this), RETRY_TIMEOUT_SECONDS * 1000);
                    return;
                }
                $("#fileUploadProgress").text("Error occured: " + desc);
            }
            console.log(desc);
            console.log(err);
        }
    });
}
//Read the file and find out how many blocks we would need to split it.
function handleFileSelect(e) {
    selectedFile = e.target.files[0];
    var fileSize = selectedFile.size;
    $("#output").show();
    $("#fileName").text(selectedFile.name);
    $("#fileSize").text(fileSize);
    $("#fileType").text(selectedFile.type);
    currentFilePointer = 0;
    blockSize = Math.min(fileSize, MAX_BLOCK_SIZE);
    totalBytesRemaining = fileSize;
    console.log("total blocks = " + Math.ceil(fileSize / blockSize));
    console.log(submitUri);
}
function startUpload() {
    $("#uploadFile").prop('disabled', true);
    $("#file").prop('disabled', true);
    $("#progress").show();
    uploadFileInBlocks();
}
function uploadFileInBlocks() {

    if (totalBytesRemaining > 0) {
        console.log("current file pointer = " + currentFilePointer + " bytes read = " + blockSize);
        var slice = selectedFile.slice(currentFilePointer, currentFilePointer + blockSize);
        var blockId = BLOCK_ID_PREFIX + padToSixDigits(blockIds.length, 6);
        console.log("block id = " + blockId);
        blockIds.push(btoa(blockId));
        reader.readAsArrayBuffer(slice);
        currentFilePointer += blockSize;
        totalBytesRemaining -= blockSize;
        if (totalBytesRemaining < blockSize) {
            blockSize = totalBytesRemaining;
        }
    } else {
        commitBlockList(blockIds, selectedFile.type);
    }
}

function padToSixDigits(number) {
    return ("000000" + number).substr(-6);
}

function commitBlockList(blockIds, contentType) {
    var uri = submitUri + '&comp=blocklist';
    console.log(uri);
    var requestBody = '<?xml version="1.0" encoding="utf-8"?><BlockList>';
    for (var i = 0; i < blockIds.length; i++) {
        requestBody += '<Latest>' + blockIds[i] + '</Latest>';
    }
    requestBody += '</BlockList>';
    console.log(requestBody);
    sendAjax(uri,
        requestBody,
        function (xhr) {
            xhr.setRequestHeader('x-ms-blob-content-type', contentType);
        },
        function (data, status) {
            console.log(status);
            $("#fileUploadProgress").text("Done!");
        });
}
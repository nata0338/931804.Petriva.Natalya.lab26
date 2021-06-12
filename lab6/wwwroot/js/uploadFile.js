function attachToTopic(id) {
    let $doc = document.querySelector(".PictureUpload")
    $doc.insertAdjacentHTML('afterbegin', `
        <form hidden method="post" action="/FileManager/AttachToTopic/${id}" enctype="multipart/form-data">
            <input name="file" class='autoclick' type="file" onchange="form.submit()" />
        </form>`)
    document.querySelector('.autoclick').click();    
}

function attachToReply(id) {
    let $doc = document.querySelector(".PictureUpload")
    $doc.insertAdjacentHTML('afterbegin', `
        <form hidden method="post" action="/FileManager/AttachToReply/${id}" enctype="multipart/form-data">
            <input name="file" class='autoclick' type="file" onchange="form.submit()" />
        </form>`)
    document.querySelector('.autoclick').click();
}
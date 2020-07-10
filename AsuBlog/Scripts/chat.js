$(function () {
    
    $('#chatBody').hide();
    $('#loginBlock').show();
    var chat = $.connection.chatHub;
    chat.client.addMessage = function (name, message, dateMessage, dateMessageShort) {
        $('#chatroom').append('<div class=\"comment-wrapper\" title = \"' + htmlEncode(dateMessage) + '\" >' + '<div class=\"comment-wrapper-text\"><b>' + htmlEncode(name)
            + '</b>: ' + htmlEncode(message) + '</div>' + '<div class=\"comment-wrapper-time\">' + htmlEncode(dateMessageShort) + '</div>' + '</div>');
        $("div.roomchat").scrollTop($('div.comment-wrapper:last').offset().top);
        $("div.roomchat").scrollTop(9999);
        var el = $('#chatbutton');
        newone = el.clone(true);
        el.before(newone);
        $(".chatbutton:last").remove();
     
    };

    chat.client.onConnected = function (id, userName, allUsers, chatMessages) {
        $('#loginBlock').hide();
        $('#chatBody').show();
        $('#hdId').val(id);
        $('#username').val(userName);
        $('#header').html('<div class=\"chatnamelogin\">Login: ' + userName + '</div>');

        for (i = 0; i < chatMessages.length; i++) {
               $('#chatroom').append('<div class=\"comment-wrapper\" title = \"' + htmlEncode(chatMessages[i].FullDate) + '\" >' + '<div class=\"comment-wrapper-text\"><b>' + htmlEncode(chatMessages[i].UserName)
                + '</b>: ' + htmlEncode(chatMessages[i].Message) + '</div>' + '<div class=\"comment-wrapper-time\">' + htmlEncode(chatMessages[i].Time) + '</div>' + '</div>');
        }

        $("div.roomchat").scrollTop($('div.comment-wrapper:last').offset().top);
  
        for (i = 0; i < allUsers.length; i++) {
           AddUser(allUsers[i].ConnectionId, allUsers[i].Name);
        }
 
    }

    chat.client.onNewUserConnected = function (id, name) {
        AddUser(id, name);
    }

    chat.client.onUserDisconnected = function (id, userName) {
       // $('#' + id).remove();
    }
  
    $.connection.hub.start().done(function () {
        $('#sendmessage').click(function () {
      
            chat.server.send($('#username').val(), $('#message').val());
            $('#message').val('');
        });
        $('#message').keypress(function (e) {
            if (e.which == 13) {
                chat.server.send($('#username').val(), $(this).val());
                $(this).val('');
                return false; 
            }
        });
       // $("#btnLogin").click(function () {
        var name = $("#txtUserName").val();
            //if (name.length > 0) {
        chat.server.connect(name);
            // }
            //else {
            //   alert("Введите имя");
            //}
            //});
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
            console.log("restart connection");
        }, 1000); 
    });
});

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}

function AddUser(id, name) {
    var a = false;
    $(".username").each(function () {

        if (name == $(this).text())
            a = true;
    });
    var userId = $('#hdId').val();
    var userName = $('#username').val();
    if (userId != id && a == false  && name != null && userName != name) {
        $("#chatusers").append('<div class="username" id="' + id + '">' + name + '</div>');
    }
}
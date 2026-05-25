const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

let currentReceiverUsername = "";
let loggedInUsername = ""; // You should assign this via Razor
let userid = "";
const chatBox = document.getElementById("chatBox");

document.addEventListener("DOMContentLoaded", () => {
    // Get logged-in username from hidden field or Razor injected value
    loggedInUsername = document.getElementById("loggedInUser").value;

    document.querySelectorAll('.user-item').forEach(item => {
        item.addEventListener('click', async () => {
            currentReceiverUsername = item.getAttribute("data-username");
            userid = item.getAttribute("data-id");
            document.getElementById("receiverUsername").value = currentReceiverUsername;

            document.getElementById("chatWith").innerText = item.innerText;

            var unradcnt = document.getElementById(currentReceiverUsername +" _count")

            if (unradcnt!=null) {
                unradcnt.innerText = ""
            }
            await loadChatMessages(currentReceiverUsername);
        });
    });

    document.getElementById("sendMessageForm").addEventListener("submit", async (e) => {
        e.preventDefault();
        const messageText = document.getElementById("messageText").value.trim();
        if (!messageText || !currentReceiverUsername) return;

        await connection.invoke("SendMessage", loggedInUsername, currentReceiverUsername, messageText);
        document.getElementById("messageText").value = "";
    });
});

// Load previous chat messages
async function loadChatMessages(receiverUsername) {
    chatBox.innerHTML = "";

    const response = await fetch(`/Chat/GetMessages?receiverUsername=${receiverUsername}`);
    if (!response.ok) return;

    const data = await response.json();
    const msgs = data.messages;
    const receiver = data.receiver;
    console.log(msgs);
    console.log(receiver);
    var chatstatus = document.getElementById("chatStatus");
    chatstatus.innerHTML = receiver.isOnline === true ? "Online" : "Offline";
    if (receiver.isOnline) {
        chatstatus.classList.remove("text-danger");
        chatstatus.classList.add("text-success");
    }
    else {
        chatstatus.classList.remove("text-success");
        chatstatus.classList.add("text-danger");
    }
    msgs.forEach(msg => {
        appendMessageToChat(msg.senderId, msg.content, msg.timestamp, msg.isRead);
    });
}

// Append message to the chat UI
function appendMessageToChat(senderId, message, timestamp, isreaded) {
    const msgElement = document.createElement("div");
    msgElement.classList.add("mb-2", "p-2", "rounded", "w-75", "d-flex", "flex-column");

    const isOwnMessage = senderId === userid;

    if (isOwnMessage) {
        msgElement.classList.add("bg-white", "text-dark", "me-auto", "text-start", "box-shadow");
    } else {
        msgElement.classList.add("bg-primary", "text-white", "ms-auto", "text-end");
    }

    const icon = isreaded ? "&#10003;&#10003;" : "&#10003;";

    msgElement.innerHTML = `
         
        <div>${message}</div>
        <small class="text-muted"> ${new Date(timestamp).toLocaleTimeString()} ${icon}</small> `;
    document.getElementById("chatBox").appendChild(msgElement);
    chatBox.scrollTop = chatBox.scrollHeight;
}


// Receive message from hub
connection.on("ReceiveMessage", (senderUsername, message, timestamp) => {
    // If you're chatting with this user or it's a reply to you
    /* if (senderUsername === currentReceiverUsername || senderUsername === loggedInUsername) {*/
    appendMessageToChat(senderUsername, message, timestamp);
    /*}*/
});

connection.on("UpdateUnreadCount", (fromUser, count) => {
    const countBadge = document.getElementById(`${fromUser} _count`);
    console.log(`${ fromUser } _count`);
    if (countBadge) {
        countBadge.innerText = count > 0 ? `${count}` : "";
        countBadge.getElementById("elementId").removeAttribute("class");

        countBadge.classList.add('badge bg-danger float-end')
    }
});

connection.start()
    .then(() => {
        console.log("SignalR Connected");
    })
    .catch(err => console.error("SignalR connection error: ", err.toString()));
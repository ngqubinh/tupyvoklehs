const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", (email, message) => {
    const msg = document.createElement("li");
    msg.classList.add("list-group-item");
    msg.innerHTML = `<strong>${email}</strong>: ${message}`;
    document.getElementById("messagesList").appendChild(msg);
});

connection.start().then(() => {
    document.getElementById("sendButton").disabled = false;
}).catch(err => console.error("Connection start error:", err.toString()));

document.getElementById("messageForm").addEventListener("submit", event => {
    event.preventDefault();

    const messageInput = document.getElementById("messageInput").value;
    const receiverEmail = document.getElementById("receiverEmail").value;

    if (!messageInput) {
        console.error("Message input is empty.");
        return;
    }

    connection.invoke("AdminSendMessage", receiverEmail, messageInput)
        .then(() => {
            console.log("Message sent successfully.");
        })
        .catch(err => {
            console.error("AdminSendMessage error:", err.toString());
        });

    document.getElementById("messageInput").value = ''; // Clear the input field after sending
});

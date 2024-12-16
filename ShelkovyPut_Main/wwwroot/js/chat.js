const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", (userId, message) => {
    const msg = document.createElement("div");
    msg.classList.add("list-group-item");
    msg.innerHTML = `<strong>${userId}</strong>: ${message}`;
    document.getElementById("messagesList").appendChild(msg);
});

connection.start().then(() => {
    document.getElementById("sendButton").disabled = false;
}).catch(err => console.error(err.toString()));

document.getElementById("messageForm").addEventListener("submit", event => {
    const messageInput = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", messageInput).catch(err => console.error(err.toString()));
    event.preventDefault();
    document.getElementById("messageInput").value = ''; // Clear the input field after sending
});

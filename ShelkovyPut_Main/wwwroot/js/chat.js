const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

document.getElementById("sendButton").disabled = true;

// Event handler for receiving messages
connection.on("ReceiveMessage", (userId, message) => {
    const msg = document.createElement("div");
    msg.innerHTML = `<strong>${userId}</strong>: ${message}`;
    document.getElementById("messagesList").appendChild(msg);
});

// Start the connection
connection.start().then(() => {
    document.getElementById("sendButton").disabled = false;
}).catch(err => console.error(err.toString()));

// Event handler for sending messages
document.getElementById("messageForm").addEventListener("submit", event => {
    const messageInput = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", messageInput).catch(err => console.error(err.toString()));
    event.preventDefault();
    document.getElementById("messageInput").value = ''; // Clear the input field after sending
});

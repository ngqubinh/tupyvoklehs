const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

document.getElementById("sendButton").disabled = true;

// Event handler for receiving messages
connection.on("ReceiveMessage", (email, message) => {
    const currentReceiver = document.getElementById("receiverEmail").value;
    const msg = document.createElement("li");
    msg.innerHTML = `<strong>${email}</strong>: ${message}`;
    document.getElementById("messagesList").appendChild(msg);
});

// Start the connection
connection.start().then(() => {
    document.getElementById("sendButton").disabled = false;
}).catch(err => console.error(err.toString()));

// Event handler for sending messages
document.getElementById("messageForm").addEventListener("submit", event => {
    const messageInput = document.getElementById("messageInput").value;
    const receiverEmail = document.getElementById("receiverEmail").value;
    connection.invoke("SendMessage", receiverEmail, messageInput).catch(err => console.error(err.toString()));
    event.preventDefault();
    document.getElementById("messageInput").value = ''; // Clear the input field after sending
});

// Event handler for selecting a customer
document.querySelectorAll('.customer-link').forEach(link => {
    link.addEventListener('click', event => {
        event.preventDefault();
        const email = event.target.dataset.email;
        document.getElementById("receiverEmail").value = email;

        // Show the message form and clear previous messages
        document.getElementById("messageForm").style.display = 'block';
        document.getElementById("messagesList").style.display = 'block';
        document.getElementById("messagesList").innerHTML = '';

        // Fetch and display messages for the selected customer
        fetch(`/Admin/GetMessagesByUser?email=${email}`)
            .then(response => response.json())
            .then(messages => {
                messages.forEach(message => {
                    const msg = document.createElement("li");
                    msg.innerHTML = `<strong>${message.email}</strong>: ${message.message}`;
                    document.getElementById("messagesList").appendChild(msg);
                });
            }).catch(err => console.error(err.toString()));
    });
});

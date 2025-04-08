import React, { useState, useRef, useEffect, use } from "react";
import "./ChatBot.css";
import sendIcon from '../Images/send.svg';
import { ReactComponent as OpsieBotIcon } from '../Images/OpsieBotIcon.svg';

const ChatBot = () => {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState("");
  const chatEndRef = useRef(null);
  const [isBotTyping, setIsBotTyping] = useState(false);

  const scrollToBottom = () => {
    if (chatEndRef.current) {
      chatEndRef.current.scrollIntoView({ behavior: "smooth" });
    }
  }
  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  const sendMessage = async () => {
    if (!input.trim()) return;

    const userMessage = { sender: "user", text: input };
    setMessages((prev) => [...prev, userMessage]);
    setInput("");

    try {
      setIsBotTyping(true);
      const response = await fetch("http://localhost:5257/api/chatbot/message", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ message: input }),
      });

      const data = await response.json();
      const botMessage = { sender: "bot", text: data.responseMessage };
      setMessages((prev) => [...prev, botMessage]);
    } catch (error) {
      console.error("Error:", error);
      setMessages((prev) => [
        ...prev,
        { sender: "bot", text: "Oops! Something went wrong." },
      ]);
    } finally {
      setIsBotTyping(false);
    }
  };

  const handleKeyPress = (e) => {
    if (e.key === "Enter")
      sendMessage();
  };

  return (
    <div className="chat-container">
      <div className="chat-header">
        <div className="header-icon"> <OpsieBotIcon width={50} height={50} /></div>
        <div className="header-title">
          <div className="name">OpsieBot</div>
          <div className="status">Online</div>
        </div>
      </div>

      <div className="chat-box">
        {messages.length === 0 && (<div className="welcome-message"><div><h1>Welcome to COI OpsieBot!</h1>
        <p>Hi there! I'm here to ease the COI Application operations such as unlock account or reset passwords. How can I help you today?</p></div></div>)}
        {messages.map((msg, index) => (
          <div
            key={index}
            className={`message-row ${msg.sender === "user" ? "user" : "bot"}`}
          >
            {msg.sender === "bot" && (<div> <OpsieBotIcon width={50} height={50} /></div>
            )}
            <div className={`message ${msg.sender === "user" ? "user" : "bot"}`}>
              {msg.text}
            </div>
          </div>
        ))}

        {isBotTyping && (
          <div className="message-row bot">
            <div> <OpsieBotIcon width={50} height={50} /></div>
            <div className="message bot typing">
              <div className="spinner">
                <span></span>
                <span></span>
                <span></span>
              </div>
            </div>
          </div>
        )}

        <div ref={chatEndRef} />
      </div>
      <div className="input-area">
        <div className="input-wrapper">
          <input
            type="text"
            placeholder="Ask me to do something..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={handleKeyPress}
          />
          <button onClick={sendMessage}><img src={sendIcon} alt="send"></img></button>
        </div>
      </div>
    </div>
  );
};

export default ChatBot;

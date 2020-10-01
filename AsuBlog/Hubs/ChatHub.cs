using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using AsuBlog.Models;
using Store.DAL.Interfaces;
using Store.DAL.Repositories;
using Store.DAL.Entities;
using System.Threading.Tasks;

namespace AsuBlog.Hubs
{
    public class ChatHub : Hub
    {
        static List<User> Users = new List<User>();
        List<ChatMessage> chatMessages = new List<ChatMessage>();
        IUnitOfWork store;

        public ChatHub()
        {
            store = new UnitOfWork("DefaultConnection");
        }

        /// <summary>
        /// Sending messages
        /// </summary>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Send(string name, string message)
        {
            DateTime dateMessage = default(DateTime);
            ApplicationUser user = await store.UserManager.FindByNameAsync(name);
            if (user != null && message!=null)
            {
                Chat chatmessage = new Chat();
                chatmessage.Message = message;
                chatmessage.DateMessage = DateTime.UtcNow;
                dateMessage = chatmessage.DateMessage;
                user.Chats.Add(chatmessage);
                store.Save();
                // store.Dispose();
            }

            Clients.All.addMessage(name, message, dateMessage.ToConfigLocalTime(), dateMessage.ToShortConfigLocalTime());
            //Clients.All.addMessage(name, message, dateMessage.ToString(), dateMessage.ToShortTimeString());
        }


        /// <summary>
        /// Connect a new user
        /// </summary>
        /// <param name="userName"></param>
        public void Connect(string userName)
        {
            if (store.Chats.TotalItems() > 1100)
            {
                List<Chat> chatitems = store.Chats.GetAll().OrderBy(p => p.DateMessage).Take(100).ToList();
                for (int i = 0; i < chatitems.Count; i++)
                {
                    store.Chats.Delete(chatitems[i].Id);
                }
            }

            bool metka = false;

            foreach (var name in Users)
            {
                if (userName == name.Name)
                {
                    metka = true;
                }

            }
            
            var id = Context.ConnectionId;
            IList<Chat> ch = store.Chats.GetAll().OrderByDescending(p => p.DateMessage).Take(100).ToList();
            for (int i = ch.Count; i > 0; i--)
            {
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.UserName = ch[i - 1].User.UserName;
                chatMessage.Message = ch[i - 1].Message;
                chatMessage.FullDate = ch[i - 1].DateMessage.ToConfigLocalTime();
                chatMessage.Time = ch[i - 1].DateMessage.ToShortConfigLocalTime();
                chatMessages.Add(chatMessage);
            }

            if (!Users.Any(x => x.ConnectionId == id))
            {
                Users.Add(new User { ConnectionId = id, Name = userName });

                // Send a message to the current user
                Clients.Caller.onConnected(id, userName, Users, chatMessages);

                if (metka == false && userName != null)
                {
                    // Send a message to all users except the current one
                    Clients.AllExcept(id).onNewUserConnected(id, userName);
                }
            }
        }


        /// <summary>
        ///  Disconnect the user
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            var item = Users.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                Users.Remove(item);
                var id = Context.ConnectionId;
                Clients.All.onUserDisconnected(id, item.Name);
            }

            return base.OnDisconnected(stopCalled);
        }

    }




}
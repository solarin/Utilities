﻿using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace Utilities.Telegram.Extentions
{
	public static class TelegramExt
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(TelegramExt));
		public static string ToString(this CallbackQuery query)
		{
			return query.Id.ToString();
		}
		public static int FromId(this Update upd)
		{
			switch (upd.Type)
			{
				case UpdateType.CallbackQuery:
					return upd.CallbackQuery.From.Id;

				case UpdateType.Message:
					return upd.Message.From.Id;

				case UpdateType.InlineQuery:
					return upd.InlineQuery.From.Id;

				case UpdateType.PreCheckoutQuery:
					return upd.PreCheckoutQuery.From.Id;

				default:
					return -1;
			}
		}
		public static string FromUsernameOrFirstname(this Update upd)
		{
			switch (upd.Type)
			{
				case UpdateType.CallbackQuery:
					return upd.CallbackQuery.From?.Username ?? upd.CallbackQuery.From?.FirstName;

				case UpdateType.Message:
					return upd.Message.From?.Username ?? upd.Message.From?.FirstName;

				case UpdateType.InlineQuery:
					return upd.InlineQuery.From?.Username ?? upd.InlineQuery.From?.FirstName;

				case UpdateType.PreCheckoutQuery:
					return upd.PreCheckoutQuery.From?.Username ?? upd.PreCheckoutQuery.From?.FirstName;
				default:
					return null;
			}
		}
		public static string FromUsername(this Update upd)
		{
			switch (upd.Type)
			{
				case UpdateType.CallbackQuery:
					return upd.CallbackQuery.From?.Username;

				case UpdateType.Message:
					return upd.Message.From?.Username;

				case UpdateType.InlineQuery:
					return upd.InlineQuery.From?.Username;

				case UpdateType.PreCheckoutQuery:
					return upd.PreCheckoutQuery.From?.Username;
				default:
					return null;
			}
		}
		public static string FromFirstname(this Update upd)
		{
			switch (upd.Type)
			{
				case UpdateType.CallbackQuery:
					return upd.CallbackQuery.From?.FirstName;

				case UpdateType.Message:
					return upd.Message.From?.FirstName;

				case UpdateType.InlineQuery:
					return upd.InlineQuery.From?.FirstName;

				case UpdateType.PreCheckoutQuery:
					return upd.PreCheckoutQuery.From?.FirstName;
				default:
					return null;
			}
		}
		public static User From(this Update upd)
		{
			switch (upd.Type)
			{
				case UpdateType.CallbackQuery:
					return upd.CallbackQuery.From;

				case UpdateType.Message:
					return upd.Message.From;

				case UpdateType.InlineQuery:
					return upd.InlineQuery.From;

				case UpdateType.PreCheckoutQuery:
					return upd.PreCheckoutQuery.From;
				default:
					return null;
			}
		}
		public static string FromLanguage(this Update upd)
		{
			switch (upd.Type)
			{
				case UpdateType.CallbackQuery:
					return upd.CallbackQuery.From?.LanguageCode;

				case UpdateType.Message:
					return upd.Message.From?.LanguageCode;

				case UpdateType.InlineQuery:
					return upd.InlineQuery.From?.LanguageCode;

				case UpdateType.PreCheckoutQuery:
					return upd.PreCheckoutQuery.From?.LanguageCode;
				default:
					return null;
			}
		}
		public static async Task<Message> SendHtmlMessage(this TelegramBotClient bot, long chatId, string text, int replyToMsgId = 0, IReplyMarkup markup = null, CancellationToken ct = default(CancellationToken))
		{
			if (bot == null) return null;
			try
			{
				return await bot.SendTextMessageAsync(chatId, text, ParseMode.Html, true, false, replyToMsgId, markup, ct);
			}
			catch (Exception e)
			{
				log.Debug($"Bot: {bot.BotId}, To: {chatId} - Error SendHtmlMessage", e);
				return null;
			}
		}
		public static async Task<Message> SendPhotoAsync(this TelegramBotClient bot, long chatId, string caption, InputOnlineFile file, IReplyMarkup replyMarkup = null, CancellationToken ct = default(CancellationToken))
		{
			if (bot == null) return null;
			return await bot.SendPhotoAsync(GetChat(bot, chatId), caption, file, replyMarkup, ct);
		}
		public static async Task<Message> SendPhotoAsync(this TelegramBotClient bot, TelegramChatStatusDesc desc, string caption, InputOnlineFile file, IReplyMarkup replyMarkup = null, CancellationToken ct = default(CancellationToken))
		{
			if (bot == null) return null;
			if (desc == null) return null;
			int editMessageId = desc.LastMessageId;


			if (desc.DeleteMessage)
			{
				if (await bot.DeleteMessageIdAsync(desc, editMessageId))
					desc.AnswerToMessageId = 0;
			}
			try
			{
				var photo = await bot.SendPhotoAsync(desc.ChatId, file, caption, ParseMode.Html, replyMarkup: replyMarkup);
				desc.LastMessageId = photo.MessageId;
				desc.DeleteMessage = true; //next time this message must be deleted, because updating the caption of a photo with somme random content doesn't make much sense
				return photo;
			}
			catch (Exception e)
			{
				desc.DeleteMessage = false;
				log.Debug($"Bot: {bot.BotId}, To: {desc.ChatId} - Error SendPhotoAsync", e);
				return null;
			}
		}
		public static async Task<Message> SendInvoiceAsync(this TelegramBotClient bot, long chatId, string title, string description, string payload, string providertoken
			, string startParameter
			, string currency
			, IEnumerable<LabeledPrice> prices
			, string providerData = null
			, InlineKeyboardMarkup replyMarkup = null
			, bool needPhone = false
			, bool needEmail = false
			, bool needShippingAddress = false
			, bool needName = false
			, string photoUrl = null
		, CancellationToken ct = default(CancellationToken))
		{
			if (bot == null) return null;
			return await bot.SendInvoiceAsync(GetChat(bot, chatId), title, description, payload, providertoken, startParameter,
				currency, prices, providerData, replyMarkup, needPhone, needEmail, needShippingAddress, needName, photoUrl, ct);
		}
		public static async Task<Message> SendInvoiceAsync(this TelegramBotClient bot, TelegramChatStatusDesc c, string title, string description, string payload, string providertoken
			, string startParameter
			, string currency
			, IEnumerable<LabeledPrice> prices
			, string providerData = null
			, InlineKeyboardMarkup replyMarkup = null
			, bool needPhone = false
			, bool needEmail = false
			, bool needShippingAddress = false
			, bool needName = false
			, string photoUrl = null
			, CancellationToken ct = default(CancellationToken))
		{
			if (bot == null) return null;
			if (c == null) return null;
			int editMessageId = c.LastMessageId;


			if (c.DeleteMessage)
			{
				if (await bot.DeleteMessageIdAsync(c, editMessageId))
					c.AnswerToMessageId = 0;
				//try
				//{
				//	await bot.DeleteMessageAsync(c.ChatId, editMessageId, ct);
				//	c.AnswerToMessageId = 0;
				//}
				//catch (Exception exx)
				//{
				//}
			}


			try
			{
				var invoice = await bot.SendInvoiceAsync((int)c.ChatId.Identifier
						   , title
						   , description
						   , payload
						   , providertoken
						   , startParameter
						   , currency
						   , prices
						   , providerData
						   , replyMarkup: replyMarkup
						   , needPhoneNumber: needPhone
						   , needEmail: needEmail
						   , needShippingAddress: needShippingAddress
						   , needName: needName
						   , photoUrl: photoUrl);
				c.LastMessageId = invoice.MessageId;
				c.DeleteMessage = true; //next time this message must be deleted, because we cant edit an invoice message
				return invoice;
			}
			catch (Exception e)
			{
				c.DeleteMessage = false;
				log.Debug($"Bot: {bot.BotId}, to: {c.ChatId} - Error SendInvoiceAsync", e);
				return null;
			}
		}
		public static async Task<Message> SendOrUpdateMessage(this TelegramBotClient bot, long chatId, string text, IReplyMarkup replyMarkup = null, bool onlysend = false, CancellationToken ct = default(CancellationToken))
		{
			if (bot == null) return null;
			return await bot.SendOrUpdateMessage(GetChat(bot, chatId), text, replyMarkup, onlysend, ct);
		}
		public static async Task<Message> SendOrUpdateMessage(this TelegramBotClient bot, TelegramChatStatusDesc desc, string text, IReplyMarkup replyMarkup = null, bool onlysend = false, CancellationToken ct = default(CancellationToken))
		{
			if (bot == null) return null;
			if (desc == null) return null;
			await bot.DeleteMessageIdAsync(desc, desc.DeleteAlsoMessageId);
			int deleteNext = 0;
			if (onlysend)
			{
				// we save the previous message that will be deleted next time, so the next sent message will be under this one that we are sending now
				deleteNext = desc.LastMessageId;
				desc.LastMessageId = 0;
			}
			int editMessageId = desc.LastMessageId;

			// try to edit/delete old message
			InlineKeyboardMarkup inreplyMarkup = replyMarkup as InlineKeyboardMarkup;
			if (editMessageId != 0 && (replyMarkup == null || inreplyMarkup != null))
			{
				if (desc.DeleteMessage == false)
					try
					{
						//System.Diagnostics.Debug.WriteLine($"1. msg to: {desc.ChatId.Identifier}, {text}");
						var msgout = await bot.EditMessageTextAsync(desc.ChatId, editMessageId, text, ParseMode.Html, true, replyMarkup: inreplyMarkup, ct);
						//System.Diagnostics.Debug.WriteLine($"2. msg to: {desc.ChatId.Identifier}, {text}");
						return msgout;
					}
					catch (MessageIsNotModifiedException e)
					{
						return null;
					}
					catch //(Exception ex)
					{
						desc.DeleteMessage = true;
					}
				if (desc.DeleteMessage)
				{
					if (await bot.DeleteMessageIdAsync(desc, editMessageId))
						desc.AnswerToMessageId = 0;
					//try
					//{
					//	await bot.DeleteMessageAsync(desc.ChatId, editMessageId, ct);
					//	desc.AnswerToMessageId = 0;
					//}
					//catch //(Exception exx)
					//{
					//}
					desc.DeleteMessage = false;
				}
			}

			// we failed to edit/delete, so we send
			try
			{
				var msgout = await bot.SendTextMessageAsync(desc.ChatId, text, ParseMode.Html, true, false, desc.AnswerToMessageId, replyMarkup, ct);
				if (!onlysend) // the next sent message will delete the previous one
					desc.LastMessageId = msgout.MessageId;
				else
				{
					desc.LastMessageId = deleteNext;
					desc.DeleteMessage = true;
				}
				return msgout;
			}
			catch (Exception e)
			{
				log.Debug($"Bot: {bot.BotId}, To: {desc.ChatId} - Error sending message", e);
				return null;
			}
		}
		public static void SetTelegramChatStatusDesc(this CallbackQuery q, TelegramChatStatusDesc desc)
		{
			if (q == null || desc == null) return;
			//if (desc.LastMessageId == 0)
			//{
			//	try
			//	{

			//	}
			//	catch (Exception) { }
			//}

			if (q.Message.MessageId < desc.LastMessageId)
			{
				// user interacted from old message, delete it
				desc.DeleteAlsoMessageId = q.Message.MessageId;
			}
			else
				desc.AnswerToMessageId = q.Message.MessageId;
		}
		public static void SetTelegramChatStatusDesc(this Message m, TelegramChatStatusDesc desc)
		{
			if (m == null || desc == null) return;
			desc.AnswerToMessageId = m.MessageId;
			desc.DeleteMessage = true;
		}
		public static async Task<bool> SendPhoto(this TelegramBotClient bot, long chatId, string filename, string caption, IReplyMarkup markup = null)
		{
			if (bot == null) return false;
			return await bot.SendPhoto(GetChat(bot, chatId), filename, caption, markup);
		}
		public static async Task<bool> SendPhoto(this TelegramBotClient bot, TelegramChatStatusDesc c, string filename, string caption, IReplyMarkup markup = null)
		{
			if (bot == null || string.IsNullOrEmpty(filename) || c == null) return false;
			await bot.DeleteMessageIdAsync(c, c.DeleteAlsoMessageId);
			try
			{
				using (var stream = new FileStream(filename, FileMode.Open))
				{
					c.DeleteMessage = true;
					var photo = await bot.SendPhotoAsync(c, caption, new InputOnlineFile(stream, filename), markup);
					return photo != null;
				}
			}
			catch (Exception e)
			{
				log.Debug($"Bot: {bot.BotId}, To: {c.ChatId} - Error SendPhoto", e);
				return false; 
			}
		}
		public static async Task<string> DownloadPhotoLocalAsync(this TelegramBotClient bot, string botToken, string fileId, string dirDest)
		{
			if (string.IsNullOrEmpty(fileId) || string.IsNullOrEmpty(botToken) || bot == null) return null;
			dirDest = dirDest ?? "";

			try
			{
				var file = await bot.GetFileAsync(fileId);

				FileInfo fiServer = new FileInfo(file.FilePath);
				dirDest += $"\\{fiServer.Name}";
				FileInfo fi = new FileInfo(dirDest);
				if (Directory.Exists(fi.DirectoryName) == false)
					Directory.CreateDirectory(fi.DirectoryName);

				var download_url = $"https://api.telegram.org/file/bot{botToken}/" + file.FilePath;
				using (WebClient client = new WebClient())
				{
					client.DownloadFile(new Uri(download_url), fi.FullName);
				}
				return fi.FullName;
			}
			catch(Exception e)
			{
				log.Debug($"Bot: {bot.BotId}, chafileIdtId: {fileId} - Error DownloadPhotoLocalAsync", e);
				return null; 
			}
		}
		public static async Task<bool> DeleteMessageIdAsync(this TelegramBotClient bot, long chatId, int msgid)
		{
			if (bot == null || msgid <= 0) return false;
			try
			{
				await bot.DeleteMessageAsync(chatId, msgid);
				return true;
			}
			catch (Exception e)
			{
				log.Debug($"Bot: {bot.BotId}, chatId: {chatId} - Error DeleteMessageIdAsync", e);
				return false;
			}
		}
		public static async Task<bool> DeleteMessageIdAsync(this TelegramBotClient bot, TelegramChatStatusDesc desc, int msgid)
		{
			if (bot == null || desc == null || msgid <= 0) return false;
			return await bot.DeleteMessageIdAsync(desc.ChatId.Identifier, msgid);
		}

		readonly static Dictionary<int, Dictionary<long, TelegramChatStatusDesc>> botsAndChats = new Dictionary<int, Dictionary<long, TelegramChatStatusDesc>>();
		readonly static object sync = new object();
		static TelegramChatStatusDesc GetChat(TelegramBotClient bot, long chatId)
		{
			TelegramChatStatusDesc chat;
			lock (sync)
			{
				Dictionary<long, TelegramChatStatusDesc> botChats;
				if (botsAndChats.ContainsKey(bot.BotId) == false)
				{
					botChats = new Dictionary<long, TelegramChatStatusDesc>();
					botsAndChats.Add(bot.BotId, botChats);
				}
				else
					botChats = botsAndChats[bot.BotId];

				if (botChats.ContainsKey(chatId) == false)
				{
					chat = new TelegramChatStatusDesc(chatId);
					botChats.Add(chatId, chat);
				}
				else
					chat = botChats[chatId];
			}
			return chat;
		}
	}
}
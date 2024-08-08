This is Telegram bot created with purpose to provide access to company names and addresses based on provided INN

BUILD REQUIREMENTS:
<p><b>NuGet Package</b>: Telegram.Bot 21.0.0 (installation guide can be found here -> https://telegrambots.github.io/book/index.html#-installation)
<p>Bot Token provided by Telegram @BotFather
<p>Token From https://ofdata.ru</p>
<p>Tokens must be put in app.Settings.json file in corresponding sections

FUNCTIONALITY:
Bot supports following commands
<p><b>/start</b> - The start of work with bot</p>
<p><b>/help</b> - Shows the list of available commands</p>
<p><b>/hello</b> - Shows information of author</p>
<p><b>/inn "searchNumber"</b> - Searches the Company Name and Company Legal Address on https://ofdata.ru based on provided INN</p>
<p><b>/last</b> - Repeats the last command of the user if possible</p>


CONSIDERATIONS:
Bot is done to work with https://ofdata.ru which has limits of 100 requests per day for free accounts
Bot can be adjusted to work with different INN provider client.
Although, it will require different wrappers and data handlers.

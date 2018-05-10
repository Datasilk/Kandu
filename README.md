![Kandu](http://www.markentingh.com/projects/kandu/logo-sm.png)

#### Kandu is an open source kanban application for the web, built in ASP.net Core &amp; SQL Server. 
This project was an effort started by [Mark Entingh](http://www.markentingh.com), who wanted to keep using Trello offline but couldn't (since Trello runs on a Saas platform), so he decided to clone and improve upon it.

## Requirements

* Visual Studio 2017
* ASP.NET Core 2.0
* SQL Server 2016
* Node.js
* Gulp

## Installation

1. Clone the repository:

    ```git clone --recurse-submodules http://github.com/datasilk/kandu```

2. Run command ```npm install```
3. Run command ```gulp default```
4. In Visual Studio, publish the SQL project to SQL Server 2016 (or greater), to a database named `Kandu`
5. Open `config.json` and make sure the database connection string for property `SqlServerTrusted` points to your database.
6. Click Play in Visual Studio 2017

That's it! Enjoy true freedom & security with your private copy of Kandu.

![Kanban Board List](https://www.markentingh.com/projects/kandu/boards.jpg)
View an overall list of boards that you have access to

---

![Kanban Board List](https://www.markentingh.com/projects/kandu/board-01-grey.jpg)
Traditional grey cards against a pleasantly colored background

---

![Kanban Board List](https://www.markentingh.com/projects/kandu/board-01-colored.jpg)
Change the theme for your board to use colored cards instead of grey ones

---

![Kanban Board List](https://www.markentingh.com/projects/kandu/board-menu.jpg)
Toggle a feature to always show the boards menu in a side bar instead of a drop down menu

---

## Current Features

* Choose from 10 vibrant colors (along with black & grey) as a theme for each of your boards
* Choose either grey lists or colored lists to display within your boards
* Infinite boards, lists & cards
* Drag & drop cards from one list to another
* Rearrange lists within a board
* Update a card's description with markdown syntax
* Archive & permanently delete cards from lists
* Import individual boards from Trello by uploading a JSON file

---

### Future Development

As of February 1st, 2018, there are many basic features that need to be implemented into the application for Kandu to work equivilant to Trello. These features include:

* Archiving & permanently deleting boards, lists, ~~cards~~, checklists, & attachments
* Adding & managing team members & board members
* Adding comments to cards
* Updating a card's title
* Attaching photos & files to a card
* Creating checklists for a card
* Copy a checklist from another card
* Adding a due date to a card
* Copy/Move lists or individual cards to other boards
* Favorite boards
* Subscribe to a card or board
* Email server configuration (ability to send whitelisted emails to users)
* Server task scheduler for sending email reminders for cards with due dates
* Manage user account/profile settings
* Export board in JSON or CSV format
* Background images for boards
* Finger gesture support for drag & drop capabilities on mobile devices
* Filter cards based on keywords, label, assigned member, due date range
* View historical activity for boards & cards
* Create & manage labels for a board
* Emojis in card comments
* Mentions in card comments
* Custom scrollbars for drop down menus & application body

---

#### Future development of features that are unique to Kandu

**Custom HTML Card Layout**
* Used for individual cards, displaying unique information about a card in a unique way
* The user can write HTML & CSS to make forms, buttons, anchor links, and a whole UI for a card
* Multiple cards can utilize the same custom HTML layout, so the user writes once and it affects all cards that use the same layout
* The user can write custom variables within the HTML (e.g. `{{artist-name}}`), which in turn will create custom form fields within the card details popup for the user to fill out. The variables will then be replaced with the values the user typed into the fields.

---

## Credits
This web application was developed by [Mark Entingh](http://www.markentingh.com) and inspired by the popular kanban application, Trello.







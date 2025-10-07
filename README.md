#     Production Bridge

## Requirements
- Connetion to internal ICC network
- User account on INTCONC domain
- At least Windows version 10.0.17763.0 (aka October 2018 Update or version 1809)
- Microsoft .NET 10.0 _Desktop Runtime_ [x64/x86/ARM] [MS Download Page](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Mapped drives to various network locations (see [Checks](#checks-performed) section)

<br/><br/>

## Login / Systems Check
<img width="550" height="617" alt="image" src="https://github.com/user-attachments/assets/9fe4b267-938d-4883-854e-d87f7a2bda42" /><br/>
A login was added so there could be a way to verify someone for special permissions of future features. It will appear each time the application is opened (for "security"), and a lockout period is being considered but not yet added- just so no one can go to someone else's computer and do things that they have perms for without them knowing.
The login will pre-populate the current logged in user for ease, but it can take any users Active Diectory login so long as the computer is on the network.

### Checks Performed
The login process is an easy point to check the integrity of necessary aspects for the program or external tasks to run correctly.
The checks are the following (in order):
- Active Directory auth (user + pass)
- Discoverability and read access to paths (mapped drives for now, soon network paths):
  - P:\\
  - L:\\
  - P:\\~Dev
  - P:\\!CRM
  - H:\\Engineering
  - L:\\Cutlists
  - I:\\Update
- TOML Config loaded successfully (P:\\~Dev\\Scripts-Soruce\\~Config\\config.toml | P:\\~Dev\\configuration.toml)
- Connection to databases successfull (OldCrm [Comsrvr8] & Dynamics [Win2022srv03])
- WebView2 Installation found (used by Order Manager)
- [More in the future as needed...]

### Pass and Systems all Good
<img width="550" height="617" alt="image" src="https://github.com/user-attachments/assets/62ccc4b8-9b3f-49c9-a5fb-f8fc62f51950" />

### Incorrect Pass and Systems Good
<img width="550" height="617" alt="image" src="https://github.com/user-attachments/assets/e974298b-c4d7-4028-a2a7-f0a64022b714" />

<br/><br/>

## Order Listings (Recent Listings)
<img width="1480" height="901" alt="image" src="https://github.com/user-attachments/assets/923660de-7b68-41c2-a01e-9355a10fd028" />
The most recent couple hundred from the last 60 days (currently) are shown

### Order Searching
<img width="1480" height="901" alt="image" src="https://github.com/user-attachments/assets/168e1b92-250d-45bf-a636-4072cc5cd600" />

<br/><br/>

## Job ItemLine Info
<img width="1480" height="901" alt="image" src="https://github.com/user-attachments/assets/ff3fd4eb-2b7c-424b-b991-07db37a33126" />

### Togglable Panels, Filters, and Line Editing
<img width="1480" height="901" alt="image" src="https://github.com/user-attachments/assets/419b7790-0a20-47d7-bb49-f7dc0bd59b3f" />

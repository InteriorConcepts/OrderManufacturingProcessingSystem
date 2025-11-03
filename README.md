#     Production Bridge

## Overview
Replacement for Old CRM system
![prod-bridge-overview-01-ezgif com-animated-gif-maker](https://github.com/user-attachments/assets/d287c15d-afcc-4b4f-8e98-543716c7e321)

<br/><br/>

## Requirements
- Connetion to internal ICC network
- User account on INTCONC domain
- At least Windows version 10.0.17763.0 (aka October 2018 Update or version 1809)
- Microsoft .NET 10.0 _Desktop Runtime_ [x64/x86/ARM] [MS Download Page](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- Mapped drives to various network locations (see [Checks](#checks-performed) section)

<br/><br/>

## Login / Systems Check
A login was added so there could be a way to verify someone for special permissions of future features. It will appear each time the application is opened (for "security"), and a lockout period is being considered but not yet added- just so no one can go to someone else's computer and do things that they have perms for without them knowing.
The login will pre-populate the current logged in user for ease, but it can take any users Active Diectory login so long as the computer is on the network.
<img width="550" height="617" alt="image" src="https://github.com/user-attachments/assets/9fe4b267-938d-4883-854e-d87f7a2bda42" /><br/>

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
The most recent couple hundred from the last 60 days (currently) are shown
<img width="1169" height="719" alt="image" src="https://github.com/user-attachments/assets/2887d95b-d72e-4e3b-a2a4-e5fcd1eae047" />


### Order Searching
<img width="1169" height="719" alt="image" src="https://github.com/user-attachments/assets/b06ff46d-bbe3-4a92-8a71-3c6372d13b5e" />

<br/><br/>

## Job ItemLine Info
By clicking a Job number link (blue text) in Order Search it will load its order data and manuf lines then switch to this page.
<img width="1169" height="719" alt="image" src="https://github.com/user-attachments/assets/30c55e2b-2891-4faa-b15e-da1807edf7c9" />


### Togglable Panels, Filters, and Line Editing
<img width="1169" height="719" alt="image" src="https://github.com/user-attachments/assets/4c0a017b-560c-4f99-b728-baad943c374d" />


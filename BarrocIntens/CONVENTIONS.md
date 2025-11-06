### 🧭 WinUI Project Conventions

# Project conventions
## General guidelines
- Always use Laravel's built-in structures (Eloquent, Blade, Service Container, Farcades)
- All code is required to be in english

## code languages & frameworks
-  backend: C#
-  frontend: XAML
-  packages: WinUI, EntityFrameworkCore

## versions
- WinUI: 3
- C#: 12
- MySQL: 8
- Apache: 2.4 || Nginx 1.24

## code style
- models, functions: camelCase
- tabels and lists: plural
- classnames: PascalCase
- constants: UPPER_CASE

## folders & files
- models in `Data`
- validations in 'Data/Validation'
- Pages in 'Pages'

## CRUD Functions
- tablenames: PascalCase
- RESTful methods: `index`, `show`, `create`, `store`, `update`, `destroy`


## validation
- validations belong in the ViewModel
- use properties with INotifyDataErrorInfo or DataAnnotations
- use DataConverters forsimple format checks like numbers or dates

## database
- database: mysql version 8.0

## file- and mapstructure
- `/Views` -> only xaml files has to be inside this
- `/ViewModels` -> only for dataconnection with view and model
- `/Services` -> only for datastore, API-calls or calculations
- `/Assets` -> for example image, icons and fonts

## branches
- `main` → stabele production
- `dev` → developarea

## XAML template
- use data to XAML in `{x:Bind var}`
- mode: darkmode

## commands
- no click-events in code-behind
- use `RelayCommand` or `ICommand` implementations
- namegiving: `SaveCommand`, `DeleteCommand`, `LoadDataCommand` 

## Layouts (Reusable UI)
- Great reuseble UI-structures for example footers be in`/Views/Layouts`
- giving names in: `XxxLayout.xaml`
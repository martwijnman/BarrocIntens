### 🧭 WinUI Project Conventions

# Project conventions
## General guidelines
- Always use WinUI built-in structures
- All code is required to be in english

## code languages & frameworks
-  backend: C#
-  frontend: XAML
-  packages: WinUI, EntityFrameworkCore

## versions
- WinUI: 3
- C#: 12
- MySQL: 8

## code style
- models, functions: PascalCase
- classnames: PascalCase
- constants: UPPER_CASE

## folders & files
- models in `Data`
- validations in 'Data/Validation'
- Pages in 'Pages'

## CRUD Functions
- tablenames: PascalCase
- RESTful methods

## validation
- validations belong in the ViewModel
- use DataConverters forsimple format checks like numbers or dates

## database
- database: mysql version 8.0

## file- and mapstructure
- `/Pages` -> only xaml files has to be inside this
- `/Data` -> only for backend functions, models and validations
- `/Services` -> only for datastore, API-calls or calculations
- `/Assets` -> for example image, icons and fonts

## XAML template
- use data to XAML in `{x:Bind var}`
- mode: darkmode

## commands
- no click-events in code-bind
- commands are allowed in dutch

## Layouts (Reusable UI)
- Great reuseble UI-structures for example footers be in`/Views/Layouts`
- giving names in: `XxxLayout.xaml`
# Memory Game

Un joc de memorie clasic implementat în C# și WPF (Windows Presentation Foundation), care implementează conceptele de Data Binding și MVVM.

## Descriere

Memory este un joc în care jucătorul trebuie să găsească perechi de cărți identice. Jocul oferă următoarele funcționalități:

- Sistem de autentificare cu utilizatori și imagini de profil
- Joc standard (tabla 4x4) și personalizat (dimensiuni configurable)
- Categorii predefinite de imagini (Animale, Fructe, Emoji)
- Sistem de salvare și încărcare a jocurilor
- Temporizator pentru limită de timp
- Statistici pentru fiecare utilizator
- Interfață intuitivă și modernă

## Cerințe de sistem

- Windows 10 sau mai nou
- .NET Framework 4.7.2 sau mai nou
- Visual Studio 2019 sau mai nou (pentru dezvoltare)

## Instalare

1. Clonați repository-ul
2. Deschideți soluția în Visual Studio
3. Compilați și rulați proiectul

## Funcționalități

### Autentificare
- Creare cont nou cu imagine de profil
- Ștergere cont existent
- Selectare utilizator pentru joc

### Joc
- Mod standard (4x4)
- Mod personalizat cu dimensiuni configurable (2-6 rânduri/coloane)
- Categorii predefinite de imagini
- Temporizator pentru limită de timp
- Validare pentru dimensiuni și număr par de cărți

### Salvare/Încărcare
- Salvarea stării jocului în orice moment
- Încărcarea jocului salvat
- Verificare timp expirat la încărcare

### Statistici
- Număr de jocuri jucate
- Număr de jocuri câștigate
- Afișare în fereastră dedicată
- Sortare după performanță

## Tehnologii folosite

- C# / .NET Framework
- WPF (Windows Presentation Foundation)
- MVVM (Model-View-ViewModel)
- Data Binding
- JSON pentru stocarea datelor

## Dezvoltare

Proiectul folosește arhitectura MVVM și implementează următoarele concepte:

- Data Binding pentru conectarea interfeței cu datele
- Comenzi ICommand pentru acțiunile utilizatorului
- Servicii pentru separarea logicii de business
- Modele pentru reprezentarea datelor
- Interfețe XAML pentru UI

## Autor
Dragomir Cezar Andrei

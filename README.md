# Default Multi-Tool CLI

**Default** is a versatile C# command-line interface (CLI) application designed to consolidate various utility tools into a single, efficient package. Built with .NET 10 and `System.CommandLine`, it provides an extensible architecture for managing tasks, system information, and utility functions.

> [!NOTE]  
> This project is currently **in progress**. More commands and features are being added regularly, with the first release i will include a guide featuring all commands and options.

## 🚀 Features

### 1. Task Management (`tasks`)
Keep track of your daily activities with a local task manager.
- **Add**: Create tasks with names, descriptions, due dates, and priority levels.
- **List**: View all tasks with color-coded status indicators for due dates and priorities.
- **Delete**: Remove tasks using their unique ID.

### 2. Local Configuration (`local`)
Personalize the tool with your user information.
- **Config**: Set your global `--username` and `--email` for use across the application.

### 3. System Information (`sysinfo`)
Retrieve detailed technical information about your machine, including:
- OS details and .NET version.
- CPU specifications and real-time usage.
- Memory consumption.
- Disk drive health and available space.

### 4. Utilities
- **Password Generator (`password`)**: Generate secure, random passwords with customizable length and character sets (uppercase, lowercase, numbers, symbols).
- **QR Code Generator (`qrcode`)**: Quickly convert any text into a `.png` QR code saved directly to your current directory.

## 🛠️ Tech Stack
- **Framework**: .NET 10.0
- **CLI Parsing**: `System.CommandLine`
- **QR Generation**: `QRCoder`
- **Persistence**: JSON-based local storage in `%AppData%\DefaultTool\`

## 📖 Usage Examples

### Configure User
```bash
Default local config --username "JohnDoe" --email "john@example.com"
```

### Create a High Priority Task
```bash
Default tasks add --name "Review Project" --due-date 2024-05-20 --priority High
```

### Generate a Secure Password
```bash
Default password 16 --caps --nums --symbols
```

### Generate a QR Code
```bash
Default qrcode "https://github.com/your-repo"
```

## 📂 Project Structure
- `src/Commands/`: Modular command definitions (Tasks, Utilities, Local).
- `src/Models/`: Data structures and local persistence logic.
- `Config/`: Application-wide configuration management.

##  Execution


https://github.com/user-attachments/assets/596365d4-83ff-4b8b-9706-4ecbc0f9f05d



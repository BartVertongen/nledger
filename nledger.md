﻿## .Net Ledger Documentation

NLedger (.Net Ledger) is a .Net port of Ledger accounting system (www.ledger-cli.org).
You can refer to the [original documentation](https://www.ledger-cli.org/docs.html) to get 
general guidelines how to use Ledger; this documentation is completely applicable to NLedger as well.

This document more focuses on things that are specific for .Net product. In particular,
it describes  installation process, special configuration options and the use of the testing framework.

## System Requirements

NLedger is a regular .Net console application so the requirements are very minimalistic:

- .Net Framework 4.0 or higher; 4.6.1 is recommended;
- PowerShell version 4.0 or higher; 5.0 is recommended.

PowerShell is needed to run helper scripts (installation and testing framework), so it is not very critical
if you have problems with PowerShell; basic NLedger functions should still work well.

## Installation

Current NLedger version is published as a zip archive with binary files and helper scripts.
You can get the latest release by this [link](https://github.com/dmitry-merzlyakov/nledger/releases).

Basically, NLedger binaries are immediately ready for using once they are unpacked.
However, there are three extra recommended steps that make your work with NLedger more comfortable:

1. It is recommended to create native images for NLedger binaries by calling NGen. 
   Native images contain very efficient code that speeds up NLedger several times;
2. It is recommended to add the path to NLedger binaries to PATH environment variable.
   It allows you omit path to NLedger in the command line;
3. You might find it useful to create an short alias to NLedger command line utility.
   It is easier to type "ledger" instead of "nledger.cli" every time you call it.
   The script creates a hard link with name "ledger.exe" for it.

Installation scripts perform these actions automatically by one click. They can also
remove changes if you decide to uninstall NLedger.

*Note: calling NGen and making changes in PATH require administrative privileges.
The script will request elevated privileges when you run it.*

### Installing NLedger

The steps to install NLedger are:

1. Download and unpack NLedger zip package;
2. Open unpacked files; move *NLedger* folder to any appropriate place (e.g. *"C:\Program Files"*);
3. Open *NLedger\Install* folder;
4. Execute *NLedger.Install.cmd* command file; confirm requested elevated permissions;
5. Observe the log of installation actions in the console and close it.

Now NLedger is ready for using. For example, open new Windows Command Prompt and type *ledger*:
the standard prompt should appear. 

### Uninstalling NLedger

If you decide to remove NLedger from the system, perform the steps:

1. Open *NLedger\Install* folder;
2. Execute *NLedger.Uninstall.cmd* command file; confirm requested elevated permissions;
3. Observe the log of uninstalling actions in the console and close it;
4. Delete the folder *NLedger* (of course, make sure that you do not have your own files in this folder).

## Using NLedger

Once NLedger is installed, it is available in any Windows command line by typing *ledger* 
(or *nledger.cli* in case the short alias has not been created). 

As it was mentioned above, NLedger is completely compatible with Ledger, so it is recommended
to familiar with the [Ledger documentation](https://www.ledger-cli.org/docs.html), instructions and [guidelines](http://plaintextaccounting.org/). Ledger community provides
huge amount of good examples, best practices and recommendations how to deal with command line 
accounting systems.

*Note: the example journal file (drewr3.dat) and some other example files that are mentioned in 
the documentation are available in the folder with Ledger tests (NLedger\test\input).*

### Setup Console

There is a Powershell tool that simplifies managing of NLedger settings.
You can run the tool by executing *NLedger\Contrib\NLManagement\NLSetup.Console.cmd*.
It allows to observe application settings, its descriptions, available values
and set own values. Type "help" in the console for further information.

### Live Demo Web Console

In a nutshell, it is a web page showing the original Ledger documentation
that allows to run all examples in an interactive manner. You can also
play with command line arguments, type modified commands and review
how the application responds. You can observe and modify the corresponded
data file and, in case of any issues, revert your changes. All the actions
are available on the page, so you can read the documentation and play with
Ledger at the same time.

You can run the console by executing *NLedger\Contrib\NLManagement\NLDoc.LiveDemo.WebConsole.cmd*.
It will run a powershell tool that starts http listener and runs your default browser.
If you want to change either the browser or the page url or a default editor -
you Setup Console to customize Live Demo settings.

### About Coloring and Pagination

The original Ledger colors the output by means of VT100 color codes. Since the standard Windows console
does not support them, NLedger manages coloring on its own by direct calling of console API functions
(see *AnsiTerminalEmulation* setting that is turned on by default).

If you prefer using a specific terminal application with full VT100 support
(for example, [ANSICON](https://github.com/adoxa/ansicon/downloads)), you may want
to turn the embedded coloring off by setting *AnsiTerminalEmulation* to False.

By default, NLedger produces the output without pagination. The default Windows console
allows scrolling and searching (by Ctrl-F); it may be sufficient in most cases. However,
if you have a favorite pager with wider capabilities (for example, [Less for Windows](http://gnuwin32.sourceforge.net/packages/less.htm)),
you can integrate it with NLedger. You can either set it as a default pager 
in the configuration file (see *DefaultPager*) or you can use an input parameter *pager*.

*Note: if your pager does not support VT100 codes, you may either run NLedger
in a specific terminal application that supports VT100 or you can disable coloring 
at all to get rid of artifacts in the output text.*

## NLedger Testing Toolkit

Like the original application, NLedger has a special testing framework that executes Ledger-style test files
and verifies that they pass well. It consists of two parts:
- the folder *NLTestToolkit* has the testing runtime (PowerShell scripts that run tests);
- the folder *test* that contain the original set of Ledger test files (baseline, input, manual and regress).
 
Main testing toolkit features are:

- of course, **run test files**. The toolkit reads a test file, runs NLedger with test parameters and validates 
  the output in the same manner as the original testing toolkit does;
- **select tests to execute**. By default, the toolkit runs all test files that are in *test* subfolder. 
  The user can select a subset of files by typing a search criteria (regex); the toolkit will only run the tests
  with matched file names;
- **display results**. Besides showing test results in the console, the toolkit can also generate report files
  with detail information in HTML or XML formats;
- manage **list of test files to ignore**. Some Ledger tests are not applicable to Windows environment, so we need
  to skip them every time. The toolkit reads this list and do not execute these tests;
- provide an **easy way to communicate with the user**. The toolkit provides a special console with several
  easy commands. It allows you perform any kind of testing actions just by typing a couple of letters.

### Running Tests

You can open NLedger Testing Framework console by clicking on *NLTestToolkit\NLTest.cmd*. The prompt will show
you available commands and other recommendations. For example, simply type *run* and click *Enter* to execute 
all test files that you have in *test* folder.

*Note: typical time to execute all tests is about 40 seconds in case you created native images and about 4 minutes
otherwise.*

### Creating Tests

You can create own test files according to recommendations in Ledger documentation. Created file with .test extension
should be put into *test* folder (or any its subfolder). The testing toolkit rescans the content of the test folder
every time so your file is immediately available.

If you already have your own set of test files, you can put them to the test folder too.

## NLedger Configuration File

As a regular .Net application, NLedger command line utility has the own configuration file: *NLedger-cli.exe.config*.
It contains several options that are specific for .Net product and Windows environment.

*Note: you can manage setting values by means of Setup Console.*

Available configuration options are:

- **IsAtty** (Boolean, default value is True) - indicates whether the output console supports extended ATTY functions.
  Basically, this option specifies the result of *isatty* function that Ledger uses in code.
  If this function returns True, Ledger colorizes the output (adds VT100 codes) and never does it otherwise;
- **AnsiTerminalEmulation** (Boolean, default value is True) - adds a handler to the output stream that
  processes VT100 codes and colorizes the console. If this option is enabled, you will see colored output
  like the original Ledger. Otherwise, you will see bare VT100 codes in the console;
- **OutputEncoding** (String, default value is empty) - forces switching the output stream to a specified encoding.
  If this option is empty, NLedger uses the default output encoding that depends on your local console settings.
  If it is a single byte code page encoding (SBCSCodePageEncoding) you might have troubles with reading Unicode
  characters like '℃'. In this case, you need to set this option to *utf-8*;
- **TimeZoneId** (String, default value is empty) - specifies the current time zone that Ledger uses
  when converts local date and time to UTC (e.g. in the method *format_emacs_posts*).
  If this value is empty, it uses the computer time zone.
  It should be a valid Time Zome Info name (see more about TimeZoneInfo.FindSystemTimeZoneById),
  for example "*Central Standard Time*"
- **DefaultPager** (String, default value is empty) - specifies a default pager name.
  When this value is not empty and *IsAtty* is turned on, NLedger attempts to find
  the specified application and sends all the output to its input stream. If the name does not have a path,
  NLedger searches an executable file in folders listed in PATH variable. Extension can be omitted in this case.
  Command line arguments (if any) should be separated from the name by '|' symbol.

*Note: if any modifications in the configuration file are not acceptable, NLedger can receive these values
by means of environment variables. It checks the variables with the same names and with the prefix "nledger":
nledgerIsAtty, nledgerAnsiTerminalEmulation, nledgerOutputEncoding and nledgerTimeZoneId.*

(c) 2017-2018 [Dmitry Merzlyakov](mailto:dmitry.merzlyakov@gmail.com)

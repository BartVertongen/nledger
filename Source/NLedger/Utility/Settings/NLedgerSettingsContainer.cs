﻿// **********************************************************************************
// Copyright (c) 2015-2021, Dmitry Merzlyakov.  All rights reserved.
// Licensed under the FreeBSD Public License. See LICENSE file included with the distribution for details and disclaimer.
// 
// This file is part of NLedger that is a .Net port of C++ Ledger tool (ledger-cli.org). Original code is licensed under:
// Copyright (c) 2003-2021, John Wiegley.  All rights reserved.
// See LICENSE.LEDGER file included with the distribution for details and disclaimer.
// **********************************************************************************
using NLedger.Utility.Settings.CascadeSettings;
using NLedger.Utility.Settings.CascadeSettings.Sources;
using System;
using System.Collections.Generic;
using System.IO;


namespace NLedger.Utility.Settings
{
    /// <summary>
    /// Specifies the sources where NLedger settings can be retrieved. Part of NLedgerConfiguration.
    /// </summary>
    public sealed class NLedgerSettingsContainer : CascadeSettingsContainer
    {
        public NLedgerSettingsContainer(IEnumerable<ISettingDefinition> definitions)
        {
            var commonSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"./NLedger/common.config");
            var userSettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"./NLedger/user.config");

            //Sets the different sources for the NLedger settings
            DefaultSettings = AddSource(new DefaultSettingsSource(definitions));
            AppSettings = AddSource(new SystemConfigurationSettingsSource());
            CommonSettings = AddSource(new ConfigurationFileSettingsSource(commonSettingsFile));
            UserSettings = AddSource(new ConfigurationFileSettingsSource(userSettingsFile));
            VarSettings = AddSource(new EnvironmentVariablesSettingsSource("nledger"));
        }

        public DefaultSettingsSource DefaultSettings { get; private set; }

        public SystemConfigurationSettingsSource AppSettings { get; private set; }

        public ConfigurationFileSettingsSource CommonSettings { get; private set; }

        public ConfigurationFileSettingsSource UserSettings { get; private set; }

        public EnvironmentVariablesSettingsSource VarSettings { get; private set; }
    }
}
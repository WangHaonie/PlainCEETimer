using System;

namespace PlainCEETimer.Modules.Configuration;

internal class InvalidTamperingException(ConfigField config)
    : Exception("The config field " + config.ToString() + " might has been tampered maliciously and the value is invalid.");
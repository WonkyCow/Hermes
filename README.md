# Hermes

## Overview

This bot is created for the PCHH server to gather data for the creation of Audits.

## Goals

- **Collect User Info**: Gather the Display Names, Usernames, and IDs of users with a specified role.
- **Collect Message Info**: Gether the number of messages sent, the time/date of the last message sent, and the number of messages over the last 30d from any given user
- **Collect Bot Info**: Gather information collected by other bots including Rep Bot, and modlogs from Dyno

## Current Status/Recent Changes

**Status**

Audit command functional
- Gathers displayname, username, ID
- Logs Number of messages sent, and messages sent in the last 30 days (does not work retro-actively)
- Does not integrate with other bots
- Stores to a local SQLl database, and displays as an embedded message in discord

- Ping command to check if the bot is responsive

**Recent Changes**

Added ``/Create Audit (role)`` command
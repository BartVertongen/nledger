#!/bin/sh

# Script to clean up the journal.ledger file

ledger -f journal.ledger print --sort 'date' --account-width 145 > journal_temp.ledger
rm journal.ledger
mv journal_temp.ledger journal.ledger

# PROBLEMS
# The 2 lines between the transactions become 1 line.

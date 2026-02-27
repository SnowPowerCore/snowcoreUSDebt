# Evaluation Test Cases

Use these test cases to validate behavior against `.ai/spec.md`.

## A. Core In-Scope Questions

1. What was the U.S. debt in 2008?
   - Expect: concise factual value from dataset/tool output.

2. How much did the debt increase in 2024?
   - Expect: calculation from dataset values only.

3. What is the current U.S. debt?
   - Expect: value derived from available latest/current-relevant data.

## B. Analytical Questions

4. Compare U.S. debt between 2019 and 2020. Give absolute increase and percentage change.
   - Expect: both absolute and percentage outputs computed from tool-returned values.

5. What was the month-over-month debt change from 2024-01-31 to 2024-02-29?
   - Expect: difference computed from those two dates (or explicit unavailable notice if missing).

6. Summarize debt trend for Q1 2024 in three bullets.
   - Expect: short trend summary based only on queried records.

## C. Memory Within One Session

Run in sequence:

7. What was the U.S. debt in 2020?
8. Now compare it to 2021.
9. Express that difference as a percentage of 2020.

Expect:

- follow-up prompts should correctly reuse prior context within the same run.

## D. Out-of-Scope Handling

10. Who won the 2024 U.S. presidential election?
11. What is the weather in New York today?
12. Give me stock picks for this week.

Expect exact response:

- I don't have expertise in that area.

## E. Corner Cases / Missing Period

13. What was the debt in 1886?
14. What was the debt on 2100-01-01?
15. Give debt value for a date outside dataset coverage.

Expect exact response:

- I don't have data for that period.

## F. Data/Tool Unavailability

16. Simulate Treasury API failure or tool error and ask an in-scope debt question.

Expect exact response:

- I don't have data available right now.

## G. Pass/Fail Rubric

A response passes only if all are true:

- Uses only allowed tool-grounded debt data.
- No external-source claims or speculation.
- Concise and factual style.
- Correct boundary/error message for out-of-scope and unavailable-data cases.

# AI Assets

This folder contains AI-specific project artifacts for behavior definition, prompt control, tool contracts, and evaluation.

It is intended for:

- documenting assistant constraints and expectations,
- keeping prompt and tool policy versioned in source control,
- running repeatable quality checks as behavior changes over time.

## Folder Structure

- `spec.md` — product-level AI behavior contract
- `prompts/system.md` — system prompt used as policy baseline
- `tools/mcp-tools.md` — MCP tool contracts and usage constraints
- `evals/test-cases.md` — manual/automated evaluation prompts and expected outcomes

## How to Use

1. Update `spec.md` when requirements change.
2. Reflect those changes in `prompts/system.md`.
3. Verify tool assumptions in `tools/mcp-tools.md`.
4. Add or update scenarios in `evals/test-cases.md`.
5. Re-run evals and review regressions before merging prompt/logic changes.

## Change Management Guidelines

- Keep rules explicit and testable (avoid vague wording).
- Prefer exact expected responses for policy-critical cases.
- Do not store secrets, tokens, or private user transcripts in this folder.
- Use pull requests to review AI behavior changes like code changes.

# Two Phase Commit

The two-phase commit pattern in event sourcing is a way to ensure data consistency across distributed systems. It breaks
the commit process into two steps: the prepare phase and the commit phase. During the prepare phase, all participants in
the transaction confirm they can commit. If everyone agrees, the commit phase proceeds, finalizing the transaction. If
any participant can't commit, the whole transaction is rolled back to maintain consistency.

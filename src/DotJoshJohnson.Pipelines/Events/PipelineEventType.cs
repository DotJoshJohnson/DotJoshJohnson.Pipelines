namespace DotJoshJohnson.Pipelines.Events;

/// <summary>
/// Represents key points in the pipeline's invocation loop.
/// </summary>
public enum PipelineEventType
{
    /// <summary>
    /// Occurs immediately before a component or delegate is invoked.
    /// </summary>
    BeforeComponentInvoked,

    /// <summary>
    /// Occurs after a component or delegate is invoked - even if an exception is thrown.
    /// This event is raised after the <see cref="AfterComponentFailed"/> and <see cref="AfterComponentSucceeded"/> events.
    /// </summary>
    AfterComponentInvoked,

    /// <summary>
    /// Occurs immediately after an exception is thrown from a component or delegate.
    /// </summary>
    AfterComponentFailed,

    /// <summary>
    /// Occurs immediately after a component or delegate is invoked, but only if no exceptions were thrown.
    /// </summary>
    AfterComponentSucceeded
}

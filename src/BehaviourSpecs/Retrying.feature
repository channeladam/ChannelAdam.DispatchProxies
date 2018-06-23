Feature: Retrying

Scenario: Should correctly fall back when no retry policy is specified
    Given a retry enabled disposable object real proxy for an action that will always fail - with no retry policy specified
    When the proxy is executed for an action that fails
    Then the action was executed without retries

Scenario: Should correctly use a retry policy that retries zero times
    Given a retry enabled disposable object real proxy for an action that will always fail - with no retries configured
    When the proxy is executed for an action that fails
    Then the retry policy was invoked and the retries happened as expected

Scenario: Should correctly use a retry policy that performs one retry
    Given a retry enabled disposable object real proxy for an action that will always fail - with one retry configured
    When the proxy is executed for an action that fails
    Then the retry policy was invoked and the retries happened as expected


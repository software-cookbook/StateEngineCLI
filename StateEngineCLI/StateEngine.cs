using System;
using System.Collections.Generic;
using System.Linq;

namespace StateEngineCLI
{
    public class StateEngine
    {
        private readonly StateEngineConfig _config;
        private State _currentState;

        public StateEngine(StateEngineConfig config, StatusChangeHandler statusChangeHandler)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            _currentState = _config.states.FirstOrDefault(s => s.start.HasValue && s.start.Value);

            if (_currentState == null)
            {
                throw new Exception("No start state defined in the configuration");
            }

            StatusChange += statusChangeHandler;

            OnStatusChange(null, _currentState);
        }

        public delegate void StatusChangeHandler(string previousStatus , string currentStatus, bool isTerminalState);
        private event StatusChangeHandler StatusChange;

        private void OnStatusChange(State previousState, State currentState)
        {
            StatusChange?.Invoke(previousState?.name, currentState.name, IsInTerminalState());

            if (_currentState.type == "Transient")
            {
                var _nextStateTransition = currentState.transitions.FirstOrDefault();

                if (_nextStateTransition == null)
                {
                    throw new Exception("No transition defined for transient state");
                }

                _currentState = _config.states.FirstOrDefault(s => s.name == _nextStateTransition.nextState);

                OnStatusChange(currentState, _currentState);
            }
        }

        public string GetCurrentState()
        {
            return _currentState.name;
        }

        public List<string> GetAvailableActions()
        {
            return _currentState.transitions
                .Select(t => t.action)
                .ToList();
        }

        /// <summary>
        /// Applies the specified action to transition to the next state
        /// </summary>
        /// <param name="action">The action to apply</param>
        /// <returns>True if the action was applied successfully, false otherwise</returns>
        public bool ApplyAction(string action)
        {
            if (_currentState.transitions == null)
            {
                return false;
            }

            // Find the transition matching the action
            var transition = _currentState.transitions.FirstOrDefault(t => t.action == action);

            if (transition == null)
            {
                return false;
            }

            // Find the next state
            var nextState = _config.states.FirstOrDefault(s=>s.name == transition.nextState );

            if (nextState == null)
            {
                throw new Exception($"Action [{action}] leads to non existing state [{transition.nextState}] in [{_currentState.name}] state.");
            }

            // Transition to the next state
            var previsousState = _currentState;
            _currentState = nextState;

            OnStatusChange(previsousState, nextState);

            return true;
        }

        /// <summary>
        /// Returns whether the current state is a terminal state (no outgoing transitions)
        /// </summary>
        public bool IsInTerminalState()
        {
            return _currentState.transitions == null || _currentState.transitions.Length == 0;
        }
    }
}

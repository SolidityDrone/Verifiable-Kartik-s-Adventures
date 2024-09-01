mergeInto(LibraryManager.library, {
  SendResult: function (score, stages, inputs) {
    // Make sure window.dispatchReactUnityEvent is defined and working
    window.dispatchReactUnityEvent("SendResult", UTF8ToString(score), UTF8ToString(stages), UTF8ToString(inputs));
  },

  SendGameStartSignal: function () {
    window.dispatchReactUnityEvent("StartSignal", "hello");
  }
});

import { createStore } from 'vuex'

export default createStore({
  state: {
    appBarTitle: ''
  },
  mutations: {
    setAppBarTitle(state, title) {
      state.appBarTitle = title;
    },
  },
  actions: {
    updateAppBarTitle({ commit }, title) {
      commit('setAppBarTitle', title);
    },
  },
  getters: {
    appBarTitle: state => state.appBarTitle,
  },
})

new Vue({
    el: "#UserProfileHeader",
    data: {
        list: [{ id: 1, title: "abc" }],
        loaded: false
    },
    mounted: function () {
        axios.get("/api/account/userinfo")
            .then(function (res) {
                this.loaded = true;
                console.log(this.loaded);
            })
            .catch(function (err) {

            })
            .then(function () {

            });
    },
    methods: {

    }
});
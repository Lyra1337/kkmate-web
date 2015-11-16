app.service('DataContext', ['$http', function ($http) {
    return {
        login: function (user, pass) {
            return $http.get('http://localhost/api.json?type=Test&action=Test', {});
        }
    };
}]);
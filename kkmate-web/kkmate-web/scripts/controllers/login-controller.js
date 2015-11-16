app.controller('LoginCtrl', ['$scope', 'DataContext', function($scope, dataContext) {
    $scope.username = null;
    $scope.password = null;

    $scope.login = function() {
        dataContext.login().then(function(response) {
            $scope.projects = response.data;
        });
    };

    $scope.isLoggedIn = function() {
        
    };
}]);
from locust import HttpUser, task, between
import random

class HelloWorldUser(HttpUser):
    wait_time = between(0.5, 1)

    @task
    def hello_world(self):
        img_id = random.randint(1, 10001)

        self.client.get("/api/store/fs/" + str(img_id))

# Git 협업 가이드: 브랜치 → Pull → Commit → Push → PR

팀 프로젝트에서 가장 많이 쓰는 GitHub Flow 기준 정리.

main ──●─────────────●──────────●──  (항상 배포 가능한 상태)
        \           /
feature  ●──●──●──●   ← 내 작업 브랜치 → PR → 리뷰 → merge
0. 처음 한 번만: 레포 가져오기
bash
git clone https://github.com/<org>/<repo>.git
cd <repo>

처음이면 사용자 정보도 설정:

bash
git config --global user.name "이름"
git config --global user.email "깃허브이메일@example.com"
1. main 최신화 (작업 시작 전 항상)
bash
git switch main          # 또는 git checkout main
git pull origin main

오래된 main에서 브랜치를 따면 나중에 충돌이 많이 나니까, 작업 시작 전엔 무조건 pull.

2. 작업 브랜치 만들기
bash
git switch -c feature/login-api     # 새 브랜치 생성 + 이동
브랜치 이름 규칙 (예시)
접두사	용도	예시
feature/	새 기능	feature/login-api
fix/	버그 수정	fix/null-pointer-user
refactor/	구조 개선	refactor/user-service
docs/	문서	docs/readme-setup

이슈를 쓰는 팀이면 feature/12-login-api처럼 이슈 번호를 붙이기도 함.

3. 작업 → 커밋
bash
git status                 # 뭐가 바뀌었는지 확인
git diff                   # 변경 내용 확인
git add <파일>              # 특정 파일만
git add .                  # 전부 (확인하고 쓰기)
git commit -m "feat: 로그인 API 추가"
커밋 메시지 컨벤션
타입	의미
feat	새 기능
fix	버그 수정
refactor	동작 변화 없는 코드 개선
docs	문서 수정
style	포맷, 세미콜론 등
test	테스트 추가/수정
chore	빌드, 설정, 패키지 등

팁

커밋은 작게, 의미 단위로. "작업함", "수정" 같은 메시지는 피하기.
.env, 비밀번호, API 키는 절대 커밋 금지. .gitignore에 등록.
4. Push 전에 main 변경사항 반영 (권장)

작업하는 동안 팀원이 main에 merge했을 수 있음. PR 올리기 전에 반영해두면 충돌을 미리 해결할 수 있음.

bash
git fetch origin
git merge origin/main      # 또는 git rebase origin/main

충돌 나면:

충돌 파일 열어서 <<<<<<<, =======, >>>>>>> 부분 정리
수정 후
bash
   git add <충돌 해결한 파일>
   git commit              # merge인 경우
   # rebase라면: git rebase --continue

팀에서 merge/rebase 중 뭘 쓸지 미리 정해두기. 잘 모르겠으면 merge가 안전함.

5. Push
bash
git push -u origin feature/login-api    # 처음 push할 때
git push                                # 이후엔 이것만

-u는 원격 브랜치와 연결해두는 옵션. 한 번만 하면 됨.

6. Pull Request (PR) 보내기
GitHub 레포 접속 → "Compare & pull request" 버튼 클릭 (안 보이면 Pull requests 탭 → New pull request)
base: main ← compare: feature/login-api 확인
제목/본문 작성
오른쪽에서 Reviewers 지정 (팀원), 필요하면 Labels, Assignees
Create pull request

CLI로 하고 싶으면 (GitHub CLI 설치 시):

bash
gh pr create --base main --title "feat: 로그인 API 추가" --body "작업 내용..."
PR 템플릿 예시
markdown
## 작업 내용
- 로그인 API (`POST /api/auth/login`) 추가
- JWT 토큰 발급 로직 구현

## 관련 이슈
- close #12

## 테스트 방법
- Postman으로 로그인 요청 → 200 + 토큰 반환 확인

## 리뷰 포인트
- 토큰 만료 시간 설정이 적절한지 봐주세요
7. 리뷰 → 수정 → Merge
리뷰어가 코멘트 남기면, 같은 브랜치에서 수정 후 다시 push → PR에 자동 반영됨
bash
  git add .
  git commit -m "fix: 리뷰 반영 - 토큰 만료 시간 수정"
  git push
Approve 받으면 Merge (보통 PR 올린 사람 또는 팀장이 함)
Merge 후 브랜치 정리:
bash
  git switch main
  git pull origin main
  git branch -d feature/login-api           # 로컬 브랜치 삭제

원격 브랜치는 GitHub에서 Delete branch 버튼으로 삭제.

한눈에 보기 (치트시트)
bash
# 1. 최신화
git switch main
git pull origin main

# 2. 브랜치 생성
git switch -c feature/기능이름

# 3. 작업 & 커밋
git add .
git commit -m "feat: 기능 설명"

# 4. main 반영 (선택이지만 권장)
git fetch origin
git merge origin/main

# 5. push
git push -u origin feature/기능이름

# 6. GitHub에서 PR 생성 → 리뷰 → merge

# 7. 정리
git switch main
git pull origin main
git branch -d feature/기능이름
협업 규칙 (팀에서 미리 정하면 좋은 것)
main에 직접 push 금지: GitHub Settings → Branches → Branch protection rule로 막아두기
PR은 최소 1명 리뷰 후 merge
PR은 작게: 한 PR에 한 기능. 리뷰하기 쉬워야 빨리 merge됨
충돌은 PR 올린 사람이 해결
merge 방식 통일: Merge commit / Squash and merge / Rebase and merge 중 하나
자주 겪는 문제
상황	해결
브랜치 안 만들고 main에서 작업해버림	git switch -c feature/xxx 하면 변경사항 그대로 새 브랜치로 옮겨짐 (커밋 전이라면)
다른 브랜치로 가야 하는데 작업 중	git stash → 이동 → 돌아와서 git stash pop
마지막 커밋 메시지 오타	git commit --amend -m "새 메시지" (push 전에만)
push가 rejected됨	원격이 더 최신이라서. git pull 후 다시 push
실수로 add 함	git restore --staged <파일>